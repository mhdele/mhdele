using LamLibAllOver;

namespace Ground.Share.Lib;

public static class Extension {
    public static async ValueTask<SResult<TOK2>> MapAsync<OK, TOK2>(this Task<SResult<OK>> task, Func<OK, Task<TOK2>> func) {
        try {
            var result = await task;
            if (result == EResult.Err) {
                return result.ChangeOkType<TOK2>();
            }

            return await result.MapAsync(func);
        }
        catch (Exception e) {
            return SResult<TOK2>.Err(e);
        }
    }
    
    public static async ValueTask<SResult<TOK2>> AndThenAsync<OK, TOK2>(this Task<SResult<OK>> task, Func<OK, ValueTask<SResult<TOK2>>> func) {
        try {
            var result = await task;
            if (result == EResult.Err) {
                return result.ChangeOkType<TOK2>();
            }

            return await result.AndThenAsync(func);
        }
        catch (Exception e) {
            return SResult<TOK2>.Err(e);
        }
    }
    
    public static async ValueTask<SResult<TOK2>> MapAsync<OK, TOK2>(this ValueTask<SResult<OK>> task, Func<OK, Task<TOK2>> func) {
        try {
            var result = await task;
            if (result == EResult.Err) {
                return result.ChangeOkType<TOK2>();
            }

            return await result.MapAsync(func);
        }
        catch (Exception e) {
            return SResult<TOK2>.Err(e);
        }
    }
    
    public static async ValueTask<SResult<TOK2>> AndThenAsync<OK, TOK2>(this ValueTask<SResult<OK>> task, Func<OK, ValueTask<SResult<TOK2>>> func) {
        try {
            var result = await task;
            if (result == EResult.Err) {
                return result.ChangeOkType<TOK2>();
            }

            return await result.AndThenAsync(func);
        }
        catch (Exception e) {
            return SResult<TOK2>.Err(e);
        }
    }
}