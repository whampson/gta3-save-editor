namespace GTA3SaveEditor.Core.Game
{
    public class VehicleModel : IdeObject
    {
        public string GameName { get; }

        public VehicleModel(short id, string modelName, string gameName)
            : base(id, modelName)
        {
            GameName = gameName;
        }

        public override string ToString()
        {
            return (Id != 0)
                ? GTA3.GetGxtString(GameName)
                : "(none)";
        }
    }
}
