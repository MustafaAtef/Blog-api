namespace BlogApi.Exceptions {
    public class UniqueEntityException : Exception {
        public string EntityName { get; }
        public string[] UniqueProps { get; }

        public UniqueEntityException(string entity, params string[] uniqueProps) {
            EntityName = entity;
            UniqueProps = uniqueProps;
        }

    }
}
