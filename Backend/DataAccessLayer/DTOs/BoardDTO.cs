using Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    public class BoardDTO
    {
        public const string NameOfBoard = "Board";
        public List<Column> columns;
        public int nextTaskId;
        private BoardController boardController;
    }
}
