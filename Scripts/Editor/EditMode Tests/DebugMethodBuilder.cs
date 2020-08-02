namespace DebugMenu.Tests
{
    public class DebugMethodBuilder : Builder<DebugMethod>
    {
        private string _name;
        private string _path;
        private object[] _parameters;
        
        public DebugMethodBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public DebugMethodBuilder WithPath(string path)
        {
            _path = path;
            return this;
        }
        
        public DebugMethodBuilder WithParameters(object[] parameters)
        {
            _parameters = parameters;
            return this;
        }

        public override DebugMethod Build()
        {
            return new DebugMethod
            {
                Name = _name,
                Path = _path,
                Parameters = _parameters
            };
        }
    }
}