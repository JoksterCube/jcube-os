# jcube-os
University assignment. Course "Operating Systems"

This project/program was created during 2018 spring semester by Jokūbas Rusakevičius.
Assignment task: Create real and virtual machines. Virtual machine memory are 16 blocks - block is 16 words. I chose 1 word 4 characters or single digit hex numbers.

Real machines has Registers:  
R1, R2 - general purpose 4 byte registers  
IC - current command index register  
PTR - page table register (user task size; max page table row length; real address block; real address cell  
SF - status flag register  
MODE - Processor mode register (User or Supervisor)  
PI - porgram interrupt register  
SI - supervisor interrupt register  
TI - timer interrupt register  

Task file structure:  
starts with $CODE  
then $DATA  
$END  

Example (output=user_input#1 + user_input#2)  
$CODE  
PDB4  
GD1  
PDB5  
GD2  
ADD  
S168  
PDB6  
HALT  
$DATA  
[40]:Input first number:  
[50]:Input second number:  
[60]:First and second number sum is:  
$END  

Commands (r=1 - R1; r=2 - R2):  
Lrxy - r = VirtualMemory(x * 10h + y)  
Srxy - VirtualMemory(x * 10h + y) = r  
INCr - r++; Updates SF register  
DECr - r--; Updates SF register  
ADD - R1 = R1 + R2; Updates SF register  
ADxy - VirtualMemory(x * 10h + y) = R1 + R2; Updates SF register  
SUB - R1 = R1 - R2; Updates SF register  
SBxy - VirtualMemory(x * 10h + y) = R1 - R2; Updates SF register  
MUL - R1 = R1 * R2; Updates SF register  
MLxy - VirtualMemory(x * 10h + y) = R1 * R2; Updates SF register  
DIV - R1 = R1 / R2; R2 = R1 % R2; Updates SF register  
CMP - Compares R1 and R2, Updates SF register  
Crxy - Compares r and VirtualMemory(x * 10h +y)  
XOR - R1 ^= R2; Updates SF register  
AND - R1 &= R2; Updates SF register  
OR - R1 |= R2; Updates SF register  
NOT - R1 = ~R1; Updates SF register  
GOxy - IC = x * 10h + y  
JGxy - if ZF = 0 and SF = 0 then GOxy  
JLxy - if SF = 1 then GOxy  
JCxy - if CF = 1 then GOxy  
JZxy - if ZF = 1 then GOxy  
JNxy - if ZF = 0 then GOxy  
GDBx - read one block from input stream and write to VirtualMemory(x * 10h + i) i = 0..10h  
GDr - read one word from input stream and write to register r  
PDBx - write one block to output stream from VirtualMemory(x * 10h + i) i = 0..10h  
PDr - write one word to output stream from register r  
FOwx - open file in mode w (w=W - write; w=R read) fileName = to VirtualMemory(x * 10h + i) i = 0..10h; R1 = FileManager file id  
FGxy - read one block from file index = x and write to VirtualMemory(x * 10h + i) i = 0..10h  
FRxy - read R2 value of words from file index = R1 and write to VirtualMemory(x * 10h + y + i) i = 0..R2  
FPxy - write one block to file index = x from VirtualMemory(x * 10h + i) i = 0..10h  
FWxy - write R2 value of words to file index = R1 from VirtualMemory(x * 10h + y + i) i = 0..R2  
FCx - close file index = x  
FCR - cloase file index = R1  
FDx - delete file, fileName = to VirtualMemory(x * 10h + i) i = 0..10h  
HALT - program end  
