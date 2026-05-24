using DAL;
using DTO;

namespace BUS
{
    public class CauHinhBUS
    {
        private CauHinhDAL dal = new CauHinhDAL();

        public CauHinhDTO GetCauHinh(string maCH)
        {
            return dal.GetCauHinh(maCH);
        }

        public bool SaveCauHinh(CauHinhDTO ch)
        {
            return dal.SaveCauHinh(ch);
        }
    }
}
