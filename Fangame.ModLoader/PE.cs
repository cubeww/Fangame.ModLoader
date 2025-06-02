#pragma warning disable CS0649

namespace Fangame.ModLoader;

using BYTE = byte;
using DWORD = int;
using LONG = int;
using WORD = short;
using ULONGLONG = long;
using System.Text;

public unsafe static class PE
{
    struct IMAGE_SECTION_HEADER
    {
        public fixed BYTE Name[8];
        public DWORD Misc;
        public DWORD VirtualAddress;
        public DWORD SizeOfRawData;
        public DWORD PointerToRawData;
        public DWORD PointerToRelocations;
        public DWORD PointerToLinenumbers;
        public WORD NumberOfRelocations;
        public WORD NumberOfLinenumbers;
        public DWORD Characteristics;
    }

    struct IMAGE_DOS_HEADER
    {
        public WORD e_magic;                     // Magic number
        public WORD e_cblp;                      // Bytes on last page of file
        public WORD e_cp;                        // Pages in file
        public WORD e_crlc;                      // Relocations
        public WORD e_cparhdr;                   // Size of header in paragraphs
        public WORD e_minalloc;                  // Minimum extra paragraphs needed
        public WORD e_maxalloc;                  // Maximum extra paragraphs needed
        public WORD e_ss;                        // Initial (relative) SS value
        public WORD e_sp;                        // Initial SP value
        public WORD e_csum;                      // Checksum
        public WORD e_ip;                        // Initial IP value
        public WORD e_cs;                        // Initial (relative) CS value
        public WORD e_lfarlc;                    // File address of relocation table
        public WORD e_ovno;                      // Overlay number
        public fixed WORD e_res[4];                    // Reserved words
        public WORD e_oemid;                     // OEM identifier (for e_oeminfo)
        public WORD e_oeminfo;                   // OEM information; e_oemid specific
        public fixed WORD e_res2[10];                  // Reserved words
        public LONG e_lfanew;                    // File address of new exe header
    }

    struct IMAGE_FILE_HEADER
    {
        public WORD Machine;
        public WORD NumberOfSections;
        public DWORD TimeDateStamp;
        public DWORD PointerToSymbolTable;
        public DWORD NumberOfSymbols;
        public WORD SizeOfOptionalHeader;
        public WORD Characteristics;
    }

    struct IMAGE_OPTIONAL_HEADER32
    {
        //
        // Standard fields.
        //

        public WORD Magic;
        public BYTE MajorLinkerVersion;
        public BYTE MinorLinkerVersion;
        public DWORD SizeOfCode;
        public DWORD SizeOfInitializedData;
        public DWORD SizeOfUninitializedData;
        public DWORD AddressOfEntryPoint;
        public DWORD BaseOfCode;
        public DWORD BaseOfData;

        //
        // NT additional fields.
        //

        public DWORD ImageBase;
        public DWORD SectionAlignment;
        public DWORD FileAlignment;
        public WORD MajorOperatingSystemVersion;
        public WORD MinorOperatingSystemVersion;
        public WORD MajorImageVersion;
        public WORD MinorImageVersion;
        public WORD MajorSubsystemVersion;
        public WORD MinorSubsystemVersion;
        public DWORD Win32VersionValue;
        public DWORD SizeOfImage;
        public DWORD SizeOfHeaders;
        public DWORD CheckSum;
        public WORD Subsystem;
        public WORD DllCharacteristics;
        public DWORD SizeOfStackReserve;
        public DWORD SizeOfStackCommit;
        public DWORD SizeOfHeapReserve;
        public DWORD SizeOfHeapCommit;
        public DWORD LoaderFlags;
        public DWORD NumberOfRvaAndSizes;
        public fixed byte DataDirectory[16 * 4];
    }

    struct IMAGE_OPTIONAL_HEADER64
    {
        public WORD Magic;
        public BYTE MajorLinkerVersion;
        public BYTE MinorLinkerVersion;
        public DWORD SizeOfCode;
        public DWORD SizeOfInitializedData;
        public DWORD SizeOfUninitializedData;
        public DWORD AddressOfEntryPoint;
        public DWORD BaseOfCode;
        public ULONGLONG ImageBase;
        public DWORD SectionAlignment;
        public DWORD FileAlignment;
        public WORD MajorOperatingSystemVersion;
        public WORD MinorOperatingSystemVersion;
        public WORD MajorImageVersion;
        public WORD MinorImageVersion;
        public WORD MajorSubsystemVersion;
        public WORD MinorSubsystemVersion;
        public DWORD Win32VersionValue;
        public DWORD SizeOfImage;
        public DWORD SizeOfHeaders;
        public DWORD CheckSum;
        public WORD Subsystem;
        public WORD DllCharacteristics;
        public ULONGLONG SizeOfStackReserve;
        public ULONGLONG SizeOfStackCommit;
        public ULONGLONG SizeOfHeapReserve;
        public ULONGLONG SizeOfHeapCommit;
        public DWORD LoaderFlags;
        public DWORD NumberOfRvaAndSizes;
        public fixed byte DataDirectory[16 * 8];
    }

    struct IMAGE_DATA_DIRECTORY
    {
        public DWORD VirtualAddress;
        public DWORD Size;
    }

    struct IMAGE_IMPORT_DESCRIPTOR
    {
        public int DUMMYUNIONNAME;
        public DWORD TimeDateStamp;                  // 0 if not bound,
                                                     // -1 if bound, and real date\time stamp
                                                     //     in IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT (new BIND)
                                                     // O.W. date/time stamp of DLL bound to (Old BIND)

        public DWORD ForwarderChain;                 // -1 if no forwarders
        public DWORD Name;
        public DWORD FirstThunk;                     // RVA to IAT (if bound this IAT has actual addresses)
    }

    static DWORD RVAToFileOffset(IMAGE_SECTION_HEADER* sections, DWORD numSections, DWORD rva)
    {
        for (DWORD i = 0; i < numSections; ++i)
        {
            DWORD va = sections[i].VirtualAddress;
            DWORD size = sections[i].Misc;
            if (rva >= va && rva < va + size)
            {
                return sections[i].PointerToRawData + (rva - va);
            }
        }
        return 0;
    }



    public static string[] GetImportNames(string path)
    {
        using var file = File.OpenRead(path);

        IMAGE_DOS_HEADER dosHeader = new IMAGE_DOS_HEADER();
        file.ReadExactly(new Span<BYTE>(&dosHeader, sizeof(IMAGE_DOS_HEADER)));
        if (dosHeader.e_magic != 0x5A4D)
        {
            throw new IOException("Not a valid PE file.");
        }

        file.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);
        DWORD peSignature;
        file.ReadExactly(new Span<BYTE>(&peSignature, sizeof(DWORD)));
        if (peSignature != 0x00004550)
        {
            throw new IOException("PE signature mismatch.");
        }

        IMAGE_FILE_HEADER fileHeader = new IMAGE_FILE_HEADER();
        file.ReadExactly(new Span<BYTE>(&fileHeader, sizeof(IMAGE_FILE_HEADER)));

        WORD magic;
        file.ReadExactly(new Span<BYTE>(&magic, sizeof(WORD)));
        file.Seek(-sizeof(short), SeekOrigin.Current);

        DWORD optHeaderSzie = fileHeader.SizeOfOptionalHeader;
        byte* optHeader = stackalloc byte[optHeaderSzie];
        file.ReadExactly(new Span<BYTE>(optHeader, optHeaderSzie));

        DWORD importRVA, importSize;
        if (magic == 0x10b)
        {
            IMAGE_OPTIONAL_HEADER32* optHeader32 = (IMAGE_OPTIONAL_HEADER32*)optHeader;
            Span<IMAGE_DATA_DIRECTORY> dataDirectory = new Span<IMAGE_DATA_DIRECTORY>(optHeader32->DataDirectory, 16);
            importRVA = dataDirectory[1].VirtualAddress;
            importSize = dataDirectory[1].Size;
        }
        else if (magic == 0x20b)
        {
            IMAGE_OPTIONAL_HEADER64* optHeader64 = (IMAGE_OPTIONAL_HEADER64*)optHeader;
            Span<IMAGE_DATA_DIRECTORY> dataDirectory = new Span<IMAGE_DATA_DIRECTORY>(optHeader64->DataDirectory, 16);
            importRVA = dataDirectory[1].VirtualAddress;
            importSize = dataDirectory[1].Size;
        }
        else
        {
            throw new IOException("Unsupported PE format.");
        }

        if (importRVA == 0 || importSize == 0)
        {
            throw new IOException("No import table found");
        }

        IMAGE_SECTION_HEADER* sections = stackalloc IMAGE_SECTION_HEADER[fileHeader.NumberOfSections];
        file.ReadExactly(new Span<BYTE>(sections, fileHeader.NumberOfSections * sizeof(IMAGE_SECTION_HEADER)));

        DWORD importOffset = RVAToFileOffset(sections, fileHeader.NumberOfSections, importRVA);
        file.Seek(importOffset, SeekOrigin.Begin);
        IMAGE_IMPORT_DESCRIPTOR importDesc;
        List<string> names = [];
        StringBuilder sb = new StringBuilder(256);
        while (true)
        {
            file.ReadExactly(new Span<BYTE>(&importDesc, sizeof(IMAGE_IMPORT_DESCRIPTOR)));
            if (importDesc.Name == 0) break;

            DWORD nameOffset = RVAToFileOffset(sections, fileHeader.NumberOfSections, importDesc.Name);
            if (nameOffset == 0) continue;

            long currentPos = file.Position;
            file.Seek(nameOffset, SeekOrigin.Begin);
            sb.Clear();
            byte b;
            while ((b = (byte)file.ReadByte()) != 0)
            {
                sb.Append((char)b);
            }
            names.Add(sb.ToString());
            file.Seek(currentPos, SeekOrigin.Begin);
        }

        return [.. names];
    }
}
