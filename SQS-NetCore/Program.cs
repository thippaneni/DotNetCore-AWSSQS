using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Net;

namespace SQS_NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "SQS-Msg-Queue";
            var queueList = GetAWSSQSQueues();
            if (queueList != null)
            {
                var queueUrl = "https://sqs.us-east-1.amazonaws.com/536344005429/" + queueName;
                var queueUrl1 = GetQueueUrl(queueName);

                if (queueList.Count == 0 || !queueList.Contains(queueUrl))
                {
                    var isQueueCreated = CreateAWSSQSQueue(queueName);
                    Console.WriteLine("Creating AWS SQS Queue, {0} .......", queueName);
                    if (isQueueCreated.HttpStatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("AWS SQS Queue Created Successfully");
                    }
                    else
                    {
                        Console.WriteLine("AWS SQS Queue Creation Failed");
                    }
                }
                else {
                    Console.WriteLine("AWS SQS already exists with name {0}", queueName);
                    Console.WriteLine("Sending message to queue - {0}", queueName);

                    var result = SendMessageToAWSQueue(queueUrl, "Hello this is test message");
                    if (result)
                    {
                        Console.WriteLine("Message  sent to queue - {0}", queueName);
                    }
                }
            }
            else {
                Console.WriteLine("Connecting to AWS SQS Queue Failed");
            }
            Console.ReadLine();
        }

        public static CreateQueueResponse CreateAWSSQSQueue(string queueName)
        {
            using (IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast1))
            {
                Console.WriteLine("AWS SQS Queue Creation Started.....");
                var createQueueReq = new CreateQueueRequest(queueName);
                return sqs.CreateQueueAsync(createQueueReq).Result;
            }
        }

        public static List<string> GetAWSSQSQueues()
        {
            using (IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast1))
            {
                Console.WriteLine("Getting list of available sqs queues .....");
                var listeQueueReq = new ListQueuesRequest();
                var listQueueRes = sqs.ListQueuesAsync(listeQueueReq).Result;
                if (listQueueRes.HttpStatusCode == HttpStatusCode.OK)
                {
                    return listQueueRes.QueueUrls;
                }
                else {
                    return null;
                }                
            }
        }

        public static bool SendMessageToAWSQueue(string queueUrl, string message)
        {
            using (IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast1))
            {
                Console.WriteLine("Adding message to queue .....");
                var sendMsgRequest = new SendMessageRequest() {
                    QueueUrl = queueUrl,
                    MessageBody = message
                };
                var sendMsgResp = sqs.SendMessageAsync(sendMsgRequest).Result;
                if (sendMsgResp.HttpStatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static string GetQueueUrl(string queueName)
        {
            using (IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast1))
            {
                return sqs.GetQueueUrlAsync(queueName).Result.QueueUrl;
            }
        }
    }
}
