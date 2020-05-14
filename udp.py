import socket
from struct import unpack

localIP = "192.168.1.19"
localPort = 7000
bufferSize = 1024
# msgFromServer = "Hello UDP Client"
# bytesToSend = str.encode(msgFromServer)

# Create a datagram socket
UDPServerSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

# Bind to address and ip
UDPServerSocket.bind((localIP, localPort))
print("UDP server up and listening")
i = 0
# Listen for incoming datagrams
while(True):
    bytesAddressPair = UDPServerSocket.recvfrom(bufferSize)
    message = bytesAddressPair[0]
    address = bytesAddressPair[1]
    clientMsg = "Message from Client:{}".format(message)
    clientIP = "Client IP Address:{}".format(address)
    if message == b'/muse/elements/blink\x00\x00\x00\x00,i\x00\x00\x00\x00\x00\x01':
        print(message)
    # if b'/muse/eeg\x00\x00\x00' in message:
    #     print(message.decode('utf-8').strip())
    # print(message)

    # UDPServerSocket.sendto(bytesToSend, address)
