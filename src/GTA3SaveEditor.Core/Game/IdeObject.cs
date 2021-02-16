﻿namespace GTA3SaveEditor.Core.Game
{
    public class IdeObject
    {
        public short Id { get; }
        public string ModelName { get; }

        public IdeObject(short id, string model)
        {
            Id = id;
            ModelName = model;
        }
    }
}
