using Kikai.BL.DTO;
using Kikai.BL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kikai.BL.Concrete
{
    public class TermRepository : IGenericRepository<TermObject>,ITermRepository
    {
        public IEnumerable<TermObject> SelectAll()
        {
            return new DataUtils().GetList<TermObject>("EXEC Term_GetAll").ToList();
        }

        public TermObject SelectByID(object id)
        {
            return new DataUtils().GetList<TermObject>("EXEC Term_GetById @TermId=@P1", id).ToList().SingleOrDefault();
        }

        public List<TermObject> SelectByOfferId(object id)
        {
            return new DataUtils().GetList<TermObject>("EXEC [Term_GetByOfferId] @OfferId=@P1", id).ToList();
        }

        /// <summary>
        /// This function return's the active term of a given offer
        /// </summary>
        /// <param name="OfferId">The offer Id</param>
        /// <returns>Active term</returns>
        public TermObject GetActiveTermForOffer(Guid OfferId)
        {
            return new DataUtils().GetList<TermObject>("EXEC Term_GetActiveTermForOffer @OfferId=@P1", OfferId).ToList().SingleOrDefault();
        }

        public void Insert(TermObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Term_Add @CPI=@P1, 
                                    @Active=@P2, 
                                    @Start=@P3, 
                                    @Expiration=@P4, 
                                    @OfferId=@P5, 
                                    @Created_By=@P6, 
                                    @Last_Updated_By=@P7",
                                    obj.CPI,
                                    obj.Active,
                                    obj.Start,
                                    obj.Expiration,
                                    obj.OfferId,
                                    obj.Created_By,
                                    obj.Last_Updated_By);
        }

        public void Update(TermObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Term_Update @Id=P1,
                                    @CPI=@P2, 
                                    @Active=@P3, 
                                    @Start=@P4, 
                                    @Expiration=@P5, 
                                    @OfferId=@P6, 
                                    @Created_By=@P7, 
                                    @Last_Updated_By=@P8",
                                    obj.Id,
                                    obj.CPI,
                                    obj.Active,
                                    obj.Start,
                                    obj.Expiration,
                                    obj.OfferId,
                                    obj.Created_By,
                                    obj.Last_Updated_By);
        }

        public void Delete(object id)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Term_Delete @Id=@P1", id);
        }


        /// <summary>
        /// Function that checks if the term has expired
        /// </summary>
        /// <param name="TermId"></param>
        /// <param name="GracePeriod"></param>
        /// <returns>
        /// Returns term if the term has not yet expired
        /// Returns null if the term has expired
        /// </returns>
        public TermObject CheckTermValidity(Guid TermId, int GracePeriod = 0)
        {
            return new DataUtils().GetList<TermObject>("EXEC Term_CheckTermValidity @TermId=@P1, @GracePeriod=@P2", TermId, GracePeriod).ToList().SingleOrDefault();
        }


        /// <summary>
        /// This function checks if a given term id exists for the given offer
        /// </summary>
        /// <param name="TermId">The term Id</param>
        /// <param name="OfferId">The offer Id</param>
        /// <returns>True if term is found for offer, False if term is not found for offer</returns>
        public bool CheckTermForOffer(Guid TermId, Guid OfferId)
        {
            if (new DataUtils().GetList<TermObject>("EXEC Term_CheckTermForOffer @TermId=@P1,@OfferId=@P2", TermId, OfferId).ToList().SingleOrDefault() != null)
                return true;
            else
                return false;
        }



        public void SetNewCPI(TermObject term)
        {
            try
            {
                new DataUtils().ExcecuteCommand(@"EXEC Term_SetNewCPI @OfferId=@P1,@UpdatedBy=@P2,@CPI=@P3", term.OfferId, term.Last_Updated_By, term.CPI);
            }
            catch
            {
                throw;
            }
            
        }

        /// <summary>
        /// Get the terms of a certain offer
        /// </summary>
        /// <param name="OfferId"></param>
        /// <returns></returns>
        public IEnumerable<TermObject> GetTermFromOfferId(Guid OfferId)
        {
            return new DataUtils().GetList<TermObject>("EXEC Term_GetTermFromOfferId @OfferId=@P1", OfferId).ToList();
        }
    }
}
