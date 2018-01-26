namespace SpeedyMVVM.DataAccess
{
    public enum RepositoryBehaviourEnum
    {
        /// <summary>
        /// The repository will be allowed to add new entities.
        /// </summary>
        Create,
        /// <summary>
        /// The repository will be allowed to query the entities collection.
        /// </summary>
        Read,
        /// <summary>
        /// The repository will be allowed to persist data or update the entity.
        /// </summary>
        Update,
        /// <summary>
        /// The repository will be allowed to remove entities.
        /// </summary>
        Delete
    }
}
