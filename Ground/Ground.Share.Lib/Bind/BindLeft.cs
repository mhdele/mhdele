namespace Ground.Share.Lib.Bind;

public abstract class BindLeft: IBind {
    public async Task UnloadAsync() { }
    
    public async Task LoadAsync() { }

    public async Task SetupAsync() { }
    
    public async Task PluginInfo() { }
}