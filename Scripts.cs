using StackExchange.Redis;

namespace RedisBloomBlazor
{
    public static class Scripts
    {
        public static LuaScript InitCms => LuaScript.Prepare(_initCms);
        private static readonly string _initCms = @"
            local sketchExists = redis.call('EXISTS', @KeyName)
            if sketchExists == 0 then
                redis.call('CMS.INITBYDIM', @KeyName, 500, 5)
            end
            return 1                 
";
    }
}