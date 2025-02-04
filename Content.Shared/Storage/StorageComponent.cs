using Content.Shared.Item;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage
{
    /// <summary>
    /// Handles generic storage with window, such as backpacks.
    /// </summary>
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class StorageComponent : Component
    {
        public static string ContainerId = "storagebase";

        // TODO: This fucking sucks
        [ViewVariables(VVAccess.ReadWrite), DataField("isOpen"), AutoNetworkedField]
        public bool IsUiOpen;

        [ViewVariables]
        public Container Container = default!;

        /// <summary>
        /// A limit for the cumulative ItemSize weights that can be inserted in this storage.
        /// If MaxSlots is not null, then this is ignored.
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
        public int MaxTotalWeight;

        /// <summary>
        /// The maximum size item that can be inserted into this storage,
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
        [Access(typeof(SharedStorageSystem))]
        public ProtoId<ItemSizePrototype>? MaxItemSize;

        /// <summary>
        /// The max number of entities that can be inserted into this storage.
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
        public int? MaxSlots;

        // TODO: Make area insert its own component.
        [DataField("quickInsert")]
        public bool QuickInsert; // Can insert storables by clicking them with the storage entity

        [DataField("clickInsert")]
        public bool ClickInsert = true; // Can insert stuff by clicking the storage entity with it

        [DataField("areaInsert")]
        public bool AreaInsert; // Clicking with the storage entity causes it to insert all nearby storables after a delay

        [DataField("areaInsertRadius")]
        public int AreaInsertRadius = 1;

        /// <summary>
        /// Whitelist for entities that can go into the storage.
        /// </summary>
        [DataField("whitelist")]
        public EntityWhitelist? Whitelist;

        /// <summary>
        /// Blacklist for entities that can go into storage.
        /// </summary>
        [DataField("blacklist")]
        public EntityWhitelist? Blacklist;

        /// <summary>
        /// Sound played whenever an entity is inserted into storage.
        /// </summary>
        [DataField("storageInsertSound")]
        public SoundSpecifier? StorageInsertSound = new SoundCollectionSpecifier("storageRustle");

        /// <summary>
        /// Sound played whenever an entity is removed from storage.
        /// </summary>
        [DataField("storageRemoveSound")]
        public SoundSpecifier? StorageRemoveSound;

        /// <summary>
        /// Sound played whenever the storage window is opened.
        /// </summary>
        [DataField("storageOpenSound")]
        public SoundSpecifier? StorageOpenSound = new SoundCollectionSpecifier("storageRustle");

        /// <summary>
        /// Sound played whenever the storage window is closed.
        /// </summary>
        [DataField("storageCloseSound")]
        public SoundSpecifier? StorageCloseSound;

        /// <summary>
        /// A dictionary storing each entity to its position within the storage grid.
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public Dictionary<EntityUid, ItemStorageLocation> StoredItems = new();

        /// <summary>
        /// A list of boxes that comprise a combined grid that determines the location that items can be stored.
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public List<Box2i> Grid = new();

        [Serializable, NetSerializable]
        public sealed class StorageInsertItemMessage : BoundUserInterfaceMessage
        {
        }

        [Serializable, NetSerializable]
        public enum StorageUiKey
        {
            Key,
        }
    }

    [Serializable, NetSerializable]
    public sealed class StorageInteractWithItemEvent : BoundUserInterfaceMessage
    {
        public readonly NetEntity InteractedItemUID;
        public StorageInteractWithItemEvent(NetEntity interactedItemUID)
        {
            InteractedItemUID = interactedItemUID;
        }
    }

    /// <summary>
    /// Network event for displaying an animation of entities flying into a storage entity
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class AnimateInsertingEntitiesEvent : EntityEventArgs
    {
        public readonly NetEntity Storage;
        public readonly List<NetEntity> StoredEntities;
        public readonly List<NetCoordinates> EntityPositions;
        public readonly List<Angle> EntityAngles;

        public AnimateInsertingEntitiesEvent(NetEntity storage, List<NetEntity> storedEntities, List<NetCoordinates> entityPositions, List<Angle> entityAngles)
        {
            Storage = storage;
            StoredEntities = storedEntities;
            EntityPositions = entityPositions;
            EntityAngles = entityAngles;
        }
    }

    [NetSerializable]
    [Serializable]
    public enum StorageVisuals : byte
    {
        Open,
        HasContents,
        StorageUsed,
        Capacity
    }
}
