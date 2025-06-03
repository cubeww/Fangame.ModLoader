using System.Diagnostics;

namespace Fangame.ModLoader.Gui;

public partial class MainForm : Form
{
    ModManager ModManager;
    string RunningDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Running");
    string ModsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Mods");
    string ExecutablePath = "";

    public MainForm()
    {
        InitializeComponent();
        Console.SetOut(new ControlWriter(OutputTextBox));
        CleanRunningDirectory();
        ModManager = new ModManager(ModsDirectory);
        ModsListBox.Items.AddRange(ModManager.GetModNames());
    }

    private void CleanRunningDirectory()
    {
        if (!Directory.Exists(RunningDirectory))
        {
            Directory.CreateDirectory(RunningDirectory);
        }

        foreach (var file in Directory.EnumerateFiles(RunningDirectory))
        {
            File.Delete(file);
        }

        foreach (var dir in Directory.EnumerateDirectories(RunningDirectory))
        {
            Directory.Delete(dir, true);
        }
    }

    private void OutputTextBox_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
            e.Effect = DragDropEffects.Copy;
    }

    private void OutputTextBox_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] files)
        {
            ExecutablePath = files[0];
            ModAndRun();
        }
    }

    private async void ModAndRun()
    {
        OutputTextBox.Clear();
        string[] modNames = ModsListBox.CheckedItems.Cast<string>().ToArray();
        string runningDirectory = Path.Combine(RunningDirectory, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        ModLoader loader = new ModLoader(ExecutablePath, runningDirectory, ModManager, modNames);
        await Task.Run(loader.ModAndRun);
    }

    private void OpenModsFolderButton_Click(object sender, EventArgs e)
    {
        Process.Start("explorer.exe", ModsDirectory);
    }

    private void ModsListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        ModConfigGrid.SelectedObject = ModManager.GetConfig((string)ModsListBox.SelectedItem!);
    }

    private void ModConfigGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        ModManager.SaveConfig((string)ModsListBox.SelectedItem!);
    }
}