// Encoder.cpp: определяет точку входа для консольного приложения.
//
#include "stdafx.h"
#include <Windows.h>
#include <winternl.h>
#include <iostream>
#include <string>
#include <fstream>
using namespace std;

int main()
{
	// Reading size of file
	FILE * file = fopen("in.exe", "rb");
	if (file == NULL) return 0;
	fseek(file, 0, SEEK_END);
	long int size = ftell(file);
	fclose(file);

	// Reading data to array of unsigned chars
	file = fopen("in.exe", "rb");
	unsigned char * in = (unsigned char *)malloc(size);
	int bytes_read = fread(in, sizeof(unsigned char), size, file);
	fclose(file);
	//FILE *fp;
	//fp = fopen("fuck.txt", "w");
	for (int i = 0; i < size; i++) {
		//fprintf(fp, "%d", i);
		in[i] = in[i] - 0x1;
		
	}
	//fclose(fp);
	file = fopen("out.exe", "wb");
	int bytes_written = fwrite(in, sizeof(unsigned char), size, file);
	fclose(file);
	
	for (int i = 0; i < size; i++) {
		in[i] = in[i] + 0x1;
	}

	file = fopen("decr.exe", "wb");
	bytes_written = fwrite(in, sizeof(unsigned char), size, file);
	fclose(file);
	return 0;
}

