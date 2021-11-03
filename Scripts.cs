using StackExchange.Redis;

namespace testRedisBloom
{
    public static class Scripts
    {
        public static LuaScript QueryCms => LuaScript.Prepare(_queryCms);
        public static readonly string _queryCms = @"
            local sketchExists = redis.call('EXISTS', @KeyName)
            if sketchExists == 0 then
                redis.call('CMS.INITBYDIM', @KeyName, 500, 5)
            end
            return redis.call('CMS.Query', @KeyName, @Item) 
        ";
        public static LuaScript IncrCms => LuaScript.Prepare(_incrCms);
        private static readonly string _incrCms = @"
            local sketchExists = redis.call('EXISTS', @KeyName)
            if sketchExists == 0 then
                redis.call('CMS.INITBYDIM', @KeyName, 500, 5)
            end
            return redis.call('CMS.INCRBY', @KeyName, @Item, 1)        
        ";
    }
}