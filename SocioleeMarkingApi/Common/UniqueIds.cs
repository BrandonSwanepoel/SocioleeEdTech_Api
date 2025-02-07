using Microsoft.EntityFrameworkCore;
using SocioleeMarkingApi.Models.Database;

namespace SocioleeMarkingApi.Common
{
	public interface IUniqueIds
	{
		Task<Guid> UniqueUserId();
		Task<Guid> UniqueEmailTrackingId();
		Task<Guid> UniquePaymentId();
		Task<Guid> UniqueContentFormId();
		Task<Guid> UniqueDesignChangeRequestId();
		Task<Guid> UniqueContentAssetId();
		Task<Guid> UniqueContentStepId();
		Task<Guid> UniqueContentFormPrimaryColorId();
		Task<Guid> UniqueContentFormExamplePhotoId();
		Task<Guid> UniquePaymentRequestUserId();
		Task<Guid> UniqueMessageId();
	}

	public class UniqueIds:IUniqueIds
	{
		private readonly SocioleeDesignContext _db;

		public UniqueIds(SocioleeDesignContext dbContext)
		{
			_db = dbContext;
		}

		public async Task<Guid> UniqueUserId()
		{
			var id = Guid.NewGuid();
			while (await _db.Users.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueEmailTrackingId()
		{
			var id = Guid.NewGuid();
			while (await _db.EmailTrackings.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniquePaymentId()
		{
			var id = Guid.NewGuid();
			while (await _db.PaymentDetails.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueContentFormId()
		{
			var id = Guid.NewGuid();
			while (await _db.ContentForms.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}


		public async Task<Guid> UniqueDesignChangeRequestId()
		{
			var id = Guid.NewGuid();
			while (await _db.DesignChangeRequests.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueContentAssetId()
		{
			var id = Guid.NewGuid();
			while (await _db.ContentAssets.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueContentStepId()
		{
			var id = Guid.NewGuid();
			while (await _db.ContentSteps.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueContentFormPrimaryColorId()
		{
			var id = Guid.NewGuid();
			while (await _db.ContentFormPrimaryColors.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueContentFormExamplePhotoId()
		{
			var id = Guid.NewGuid();
			while (await _db.ContentFormExamplePhotos.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniquePaymentRequestUserId()
		{
			var id = Guid.NewGuid();
			while (await _db.PaymentRequests.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

		public async Task<Guid> UniqueMessageId()
		{
			var id = Guid.NewGuid();
			while (await _db.Messages.AnyAsync(x => x.Id == id))
			{
				id = Guid.NewGuid();
			}
			return id;
		}

	}
}

