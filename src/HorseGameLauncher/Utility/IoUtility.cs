using System;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Serilog;
using SharpCompress.Compressors.Xz;

namespace HorseGameLauncher.Utility;

internal static class IoUtility
{
    internal static async Task ExtractTarGz(string filePath, string extractToPath)
    {
        try
        {
            string decompressedFilePath = await DecompressGz(filePath);
            await ExtractTar(decompressedFilePath, extractToPath);

            File.Delete(decompressedFilePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to extract {FilePath}", filePath);
        }
    }

    internal static async Task ExtractTarXz(string filePath, string extractToPath)
    {
        try
        {
            string decompressedFilePath = await DecompressXz(filePath);
            await ExtractTar(decompressedFilePath, extractToPath);

            File.Delete(decompressedFilePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to extract {FilePath}", filePath);
        }
    }

    private static async Task<string> DecompressXz(string filePath)
    {
        string cacheDirectory = PathUtility.GetCachePath();
        string fileName = Path.GetFileName(filePath);
        string decompressedFilePath = Path.Combine(cacheDirectory, fileName.Replace(".xz", ""));

        await using FileStream compressedFs = File.Open(filePath, FileMode.Open);
        await using FileStream outputFs = File.Create(decompressedFilePath);
        await using XZStream decompressor = new XZStream(compressedFs);

        await decompressor.CopyToAsync(outputFs);

        return decompressedFilePath;
    }

    private static async Task<string> DecompressGz(string filePath)
    {
        string cacheDirectory = PathUtility.GetCachePath();
        string fileName = Path.GetFileName(filePath);
        string decompressedFilePath = Path.Combine(cacheDirectory, fileName.Replace(".gz", ""));

        await using FileStream compressedFs = File.Open(filePath, FileMode.Open);
        await using FileStream outputFs = File.Create(decompressedFilePath);
        await using GZipStream decompressor = new(compressedFs, CompressionMode.Decompress);

        await decompressor.CopyToAsync(outputFs);

        return decompressedFilePath;
    }

    private static async Task ExtractTar(string filePath, string destinationPath)
    {
        Directory.CreateDirectory(destinationPath);

        await using FileStream fs = File.Open(filePath, FileMode.Open);
        await TarFile.ExtractToDirectoryAsync(fs, destinationPath, true);
    }
}