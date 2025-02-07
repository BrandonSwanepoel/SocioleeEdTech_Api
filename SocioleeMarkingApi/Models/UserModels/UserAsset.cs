namespace SocioleeMarkingApi.Models
{
	public class UserAsset
	{
        public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public string Type{ get; set; }

		public UserAsset(Guid id, Guid userId, string type)
        {
            Id = id;
            UserId = userId;
            Type = type;
        }
    }
}

