using System.Collections.Generic;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class Training
    {
        public int Id { set; get; }
        
        public IList<PhysicalState> PhysicalStates { set; get; }
    }
}
