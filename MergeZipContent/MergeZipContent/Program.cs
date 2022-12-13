
using Sharprompt;
using System.IO.Compression;

string zipFilePath = default!;
while (true)
{
    zipFilePath = args.Any()
        ? args[0]
        : Prompt.Input<string>("zipファイルを指定してください。");
    if (zipFilePath.EndsWith("zip") && File.Exists(zipFilePath))
    {
        break;
    }
}

string destFilePath = $"{zipFilePath}.txt";
if (File.Exists(destFilePath))
{
    var answer = Prompt.Confirm($"{destFilePath} が存在します。削除してもよろしいですか？", defaultValue: true);
    if (!answer)
    {
        Console.WriteLine("処理を中断します。");
        return;
    }
}

var extractDir = GetTemporaryDirectory();
try
{
    ZipFile.ExtractToDirectory(zipFilePath, extractDir);

    using (File.Create(destFilePath))
    {
    }

    foreach (var file in Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories))
    {
        File.AppendAllText(destFilePath, File.ReadAllText(file));
        File.AppendAllLines(destFilePath, new []{ string.Empty, string.Empty});
    }
}
finally
{
    try
    {
        Directory.Delete(extractDir, true);
    }
    catch
    {
        // ignore
    }
}


static string GetTemporaryDirectory()
{
    string tempDirectory = Path.GetTempFileName();
    File.Delete(tempDirectory);
    Directory.CreateDirectory(tempDirectory);
    return tempDirectory;
}
