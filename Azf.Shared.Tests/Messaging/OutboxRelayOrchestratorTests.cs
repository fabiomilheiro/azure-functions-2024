//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azf.Shared.Tests.Messaging;

//public class OutboxRelayOrchestratorTests
//{
//    private readonly TestApp app;
//    private readonly IOutboxRelayer sut;

//    public OutboxRelayOrchestratorTests()
//    {
//        this.app = new TestApp();
//        this.sut = this.app.GetService<IOutboxRelayer>();
//    }

//    [Fact]
//    public async Task RelayMessagesAsync_NoMessages_DoesNothing()
//    {
//        await this.sut.RelayMessagesAsync();

//        this.app.Db.OutboxMessages.Should().BeEmpty();
//    }

//    [Theory]
//    [InlineData(OutboxMessageState.Processing)]
//    [InlineData(OutboxMessageState.MaxAttemptsReached)]
//    public async Task RelayMessagesAsync_HasMessagesNotWaiting_DoesNothing(OutboxMessageState state)
//    {
//        var message1 = this.CreateQueueMessage("1");
//        message1.State = state;
//        this.app.Db.OutboxMessages.Add(message1);
//        await this.app.Db.SaveChangesAsync();

//        await this.sut.RelayMessagesAsync();

//        this.app.Db.OutboxMessages.Should().BeEquivalentTo(
//            new[]
//            {
//                message1,
//            });
//    }

//    [Fact]
//    public async Task RelayMessagesAsync_HasReadyMessages_RelaysMessagesToServiceBus()
//    {
//        var message1 = this.CreateQueueMessage("1");
//        var message2 = this.CreateQueueMessage("2");
//        this.app.Db.OutboxMessages.AddRange(message1, message2);
//        await this.app.Db.SaveChangesAsync();
//        this.app.QueueClient.SendMessageRequests.Clear();

//        await this.sut.RelayMessagesAsync();

//        this.app.Db.OutboxMessages.Should().BeEmpty();
//        this.app.QueueClient.SendMessageRequests.Should()
//            .BeEquivalentTo(
//                new[] {
//                    new TestMessage1
//                    {
//                        MessageId = "1",
//                        TestValue = "Value-1",
//                    },
//                    new TestMessage1
//                    {
//                        MessageId = "2",
//                        TestValue = "Value-2",
//                    },
//                });
//    }

//    [Fact]
//    public async Task RelayMessagesAsync_FailsAFewTimes_RelaysMessagesToServiceBus()
//    {
//        var message1 = this.CreateQueueMessage("1");
//        this.app.Db.OutboxMessages.AddRange(message1);
//        await this.app.Db.SaveChangesAsync();
//        this.app.QueueClient.SendMessageRequests.Clear();
//        this.app.QueueClient.ExceptionsLeft = 2;

//        await this.sut.RelayMessagesAsync();
//        await this.sut.RelayMessagesAsync();
//        await this.sut.RelayMessagesAsync();

//        this.app.Db.OutboxMessages.Should().BeEmpty();
//        this.app.QueueClient.SendMessageRequests.Should()
//           .BeEquivalentTo(
//               new[]
//               {
//                   new TestMessage1
//                   {
//                       MessageId = "1",
//                       TestValue = "Value-1",
//                   },
//               });
//    }

//    [Fact]
//    public async Task RelayMessagesAsync_FailsTooManyTimes_GivesUp()
//    {
//        var message1 = this.CreateQueueMessage("1");
//        this.app.Db.OutboxMessages.AddRange(message1);
//        await this.app.Db.SaveChangesAsync();
//        this.app.QueueClient.SendMessageRequests.Clear();
//        this.app.QueueClient.ExceptionsLeft = 100;

//        for (var i = 1; i <= OutboxRelayer.MaxNumberOfAttempts + 1; i++)
//        {
//            await this.sut.RelayMessagesAsync();
//        }

//        message1.State.Should().Be(OutboxMessageState.MaxAttemptsReached);
//        this.app.Db.OutboxMessages.Should().BeEquivalentTo(
//            new[]
//            {
//                message1,
//            });
//        this.app.QueueClient.SendMessageRequests.Should().BeEmpty();
//    }

//    private QueueMessage CreateQueueMessage(string seed)
//    {
//        return new QueueMessage
//        {
//            MessageId = seed,
//            Request = this.app.JsonService.Serialize(new TestMessage1
//            {
//                MessageId = seed,
//                TestValue = $"Value-{seed}",
//            }),
//            RequestTypeName = nameof(TestMessage1),
//        };
//    }
//}