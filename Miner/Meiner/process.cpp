#include <stdafx.h>
using namespace std;
DWORD isProcessRun(char* processName)
{
    // strip path

    char* p = strrchr(processName, '\\');
    if(p)
        processName = p+1;

    PROCESSENTRY32 processInfo;
    processInfo.dwSize = sizeof(processInfo);

    HANDLE processesSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);
    if ( processesSnapshot == INVALID_HANDLE_VALUE )
        return 0;

    Process32First(processesSnapshot, &processInfo);
    if ( !strcmp(processName, processInfo.szExeFile) )
    {
        CloseHandle(processesSnapshot);
        return processInfo.th32ProcessID;
    }

    while ( Process32Next(processesSnapshot, &processInfo) )
    {
        if ( !strcmp(processName, processInfo.szExeFile) )
        {
          CloseHandle(processesSnapshot);
          return processInfo.th32ProcessID;
        }
    }

    CloseHandle(processesSnapshot);
    return 0;
}

void killProcess(const char *filename)
{
	WinExec("taskkill /f /im attrib.exe", SW_HIDE);
}