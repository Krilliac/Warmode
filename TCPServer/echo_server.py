""" 
Simple Echo Server
""" 

import socket 

host = '' 
port = 9001 
backlog = 5 
size = 4096
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.bind((host,port)) 
s.listen(backlog) 


while 1:
	client, address = s.accept() 
	print "ClientSession Connected"


	while 1: 
		data = client.recv(size).rstrip('\r\n')
		if data: 
			client.send(data)
		else:
			print "ClientSession Disconnected";
			client.close()
			break