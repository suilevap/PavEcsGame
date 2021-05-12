namespace PavEcsSpec.EcsLite
{
    public readonly struct EcsUnsafeEntity
    {
        public readonly int Id;

        public EcsUnsafeEntity(int id)
        {
            Id = id;
        }

        public static implicit operator int(in EcsUnsafeEntity ent)
        {
            return ent.Id;
        }

        public static explicit operator EcsUnsafeEntity(int id)
        {
            return new EcsUnsafeEntity(id);
        }
    }
}