using Chip.Net.Controllers.Distributed.Models;
using Chip.Net.Controllers.Distributed.Services.ModelTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chip.Net.UnitTests.Controllers.Distributed
{
	public class TestModel : IDistributedModel {
		public int Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}

	[TestClass]
    public class ModelTrackerCollectionTests
    {
		public ModelTrackerCollection<TestModel> Models { get; set; }

		[TestInitialize]
		public void Initialize() {
			Models = new ModelTrackerCollection<TestModel>();
		}

		public void ModelTrackerCollection_NewCollection_AddModel_CountIncreased() {

		}

		public void ModelTrackerCollection_NewCollection_AddModel_ContainsModel_ReturnsTrue() {

		}

		public void ModelTrackerCollection_NewCollection_AddModel_IndexReturnsModel() {

		}

		public void ModelTrackerCollection_NewCollection_AddRmoveModel_CountUnchanged() {

		}

		public void ModelTrackerCollection_NewCollection_AddRemoveModel_ContainsModel_ReturnsFalse() {

		}

		public void ModelTrackerCollection_NewCollection_AddRemoveModel_IdRecycled() {

		}

		public void ModelTrackerCollection_NewCollection_CopyToArray_ArrayContainsModels() {

		}

		public void ModelTrackerCollection_NewCollection_ClearCollection_CollectionEmpty() {

		}

		public void ModelTrackerCollection_NewCollection_DisposeCollection_CollectionDisposed() {

		}

		public void ModelTrackerCollection_NewCollection_GetEnumerator_ReturnsEnumerator() {

		}

		public void ModelTrackerCollection_NewCollection_GetFromIndex_ReturnsNull() {

		}

		public void ModelTrackerCollection_ExistingCollection_AddModel_CountIncreased() {

		}

		public void ModelTrackerCollection_ExistingCollection_AddModel_ContainsModel_ReturnsTrue() {

		}

		public void ModelTrackerCollection_ExistingCollection_AddModel_IndexReturnsModel() {

		}

		public void ModelTrackerCollection_ExistingCollection_AddRmoveModel_CountUnchanged() {

		}

		public void ModelTrackerCollection_ExistingCollection_AddRemoveModel_ContainsModel_ReturnsFalse() {

		}

		public void ModelTrackerCollection_ExistingCollection_AddRemoveModel_IdRecycled() {

		}

		public void ModelTrackerCollection_ExistingCollection_CopyToArray_ArrayContainsModels() {

		}

		public void ModelTrackerCollection_ExistingCollection_ClearCollection_CollectionEmpty() {

		}

		public void ModelTrackerCollection_ExistingCollection_DisposeCollection_CollectionDisposed() {

		}

		public void ModelTrackerCollection_ExistingCollection_GetEnumerator_ReturnsEnumerator() {

		}

		public void ModelTrackerCollection_DisposedCollection_AddModel_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_RemoveModel_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_Contains_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_Clear_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_CopyTo_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_GetEnumerator_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_GetCount_ThrowsException() {

		}

		public void ModelTrackerCollection_DisposedCollection_Dispose_ThrowsException() {

		}
	}
}
