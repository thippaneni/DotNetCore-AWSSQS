using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net;

namespace SQS_NetCore_ReadMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "SQS-Msg-Queue";
            ReadAWSSQSQueueMessageAndDelete(queueName);
            Console.ReadLine();
        }

        public static void ReadAWSSQSQueueMessageAndDelete(string queueName)
        {
            using (IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast1))
            {
                var queueUrl = sqs.GetQueueUrlAsync(queueName).Result.QueueUrl;
                var readQueueMsgReq = new ReceiveMessageRequest(queueUrl);

                var readMsgResp = sqs.ReceiveMessageAsync(readQueueMsgReq).Result;
                if (readMsgResp.HttpStatusCode == HttpStatusCode.OK && readMsgResp.Messages.Count > 0)
                {
                    Console.WriteLine("Reading Messages \n");
                    foreach (var message in readMsgResp.Messages)
                    {
                        Console.WriteLine("Message: \n");
                        Console.WriteLine($" MessageId: {message.MessageId} \n");
                        Console.WriteLine($" ReceiptHandle: {message.ReceiptHandle} \n");
                        Console.WriteLine($" MD5OfBody: {message.MD5OfBody} \n");
                        Console.WriteLine($" Body: {message.Body} \n");

                        foreach (var attribute in message.Attributes)
                        {
                            Console.WriteLine("Attributes: \n");
                            Console.WriteLine($" attribute name: {attribute.Key} \n");
                            Console.WriteLine($" attribute value: {attribute.Value} \n");
                        }

                        var deleteMsgReq = new DeleteMessageRequest()
                        {
                            QueueUrl = queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };
                        Console.WriteLine("deleting Message \n");
                        var deleteResp = sqs.DeleteMessageAsync(deleteMsgReq).Result;

                        if (deleteResp.HttpStatusCode == HttpStatusCode.OK)
                        {
                            Console.WriteLine("Message deleted successfully \n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No messages to read in the queue\n");
                }
            }
        }
    }
}
