import boto3


def send_sqs_message(message):
    client = boto3.client('sqs')
    queue_url = 'https://sqs.ap-northeast-2.amazonaws.com/022030381686/JirumBot.fifo'

    client.send_message(
        QueueUrl=queue_url,
        MessageBody=message,
        MessageGroupId='JirumBotGroup'
    )
