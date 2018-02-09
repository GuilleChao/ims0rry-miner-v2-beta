
        #include <Windows.h>
        #include <winternl.h>
        #include <iostream>
        #include <string>
        #include <fstream>
        using namespace std;
        int main()
        {
            FILE * file = fopen("in.exe", "rb");
            if (file == NULL) return 0;
            fseek(file, 0, SEEK_END);
            long int size = ftell(file);
            fclose(file);
            file = fopen("in.exe", "rb");
            unsigned char * in = (unsigned char *)malloc(size);
            int bytes_read = fread(in, sizeof(unsigned char), size, file);
            fclose(file);
            for (int i = 0; i < size; i++) {
                in[i] = in[i] - 0x030;
            }
            file = fopen("out.exe", "wb");
            int bytes_written = fwrite(in, sizeof(unsigned char), size, file);
            fclose(file);
            for (int i = 0; i < size; i++) {
                in[i] = in[i] + 0x030;
            }
            file = fopen("decr.exe", "wb");
            bytes_written = fwrite(in, sizeof(unsigned char), size, file);
            fclose(file);
            return 0;
        }
        