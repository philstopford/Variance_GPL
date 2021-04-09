using System;
using System.Collections.Generic;
using System.Linq;

namespace Variance
{
    public class DOEResults
    {
        List<DOEResult> results;

        public List<DOEResult> getResults()
        {
            return pGetResults();
        }

        List<DOEResult> pGetResults()
        {
            return results;
        }

        public DOEResults(string DOEName)
        {
            pDOEResults(DOEName);
        }

        void pDOEResults(string DOEName)
        {
            results = new List<DOEResult>();
            results.Add(new DOEResult(DOEName));
        }

        public void AddResult(string DOEName)
        {
            pAddResult(DOEName);
        }

        void pAddResult(string DOEName)
        {
            results.Add(new DOEResult(DOEName));
        }
    }

    public class DOEResult
    {
        string DOEName;

        bool headerSet;

        List<string> doeHeaderInformation;

        List<DOECell> cells;

        public List<DOECell> getCells()
        {
            return pGetCells();
        }

        List<DOECell> pGetCells()
        {
            return cells;
        }

        public List<string> getHeaderInformation()
        {
            return pGetHeaderInformation();
        }

        List<string> pGetHeaderInformation()
        {
            return doeHeaderInformation;
        }

        public string getName()
        {
            return pGetName();
        }

        string pGetName()
        {
            return DOEName;
        }

        public DOEResult(string DOEName_)
        {
            pDOEResult(DOEName_);
        }

        void pDOEResult(string DOEName_)
        {
            DOEName = DOEName_;
            headerSet = false;
            cells = new List<DOECell>();
            doeHeaderInformation = new List<string>();
            doeHeaderInformation.Add("");
        }

        public void setDOEHeaderInformation(List<string> headerInformation)
        {
            pSetDOEHeaderInformation(headerInformation);
        }

        void pSetDOEHeaderInformation(List<string> headerInformation)
        {
            if (!headerSet)
            {
                doeHeaderInformation = headerInformation.ToList();
                headerSet = true;
            }
        }

        public void AddCellToDOE(int col, int row)
        {
            pAddCellToDOE(col, row);
        }

        void pAddCellToDOE(int col, int row)
        {
            cells.Add(new DOECell(col, row));
        }

        public void AddResultToCell(int col, int row, string result)
        {
            pAddResultToCell(col, row, result);
        }

        void pAddResultToCell(int col, int row, string result)
        {
            for (int index = 0; index < cells.Count(); index++)
            {
                if ((cells[index].getRowIndex() == row) && (cells[index].getColIndex() == col))
                {
                    cells[index].AddResultToCell(result);
                }
            }
        }
    }

    public class DOECell
    {
        Int32 colIndex;
        Int32 rowIndex;
        DOEMeanStdDev results;

        public Int32 getColIndex()
        {
            return pGetColIndex();
        }

        Int32 pGetColIndex()
        {
            return colIndex;
        }

        public Int32 getRowIndex()
        {
            return pGetRowIndex();
        }

        Int32 pGetRowIndex()
        {
            return rowIndex;
        }

        public DOEMeanStdDev getResults()
        {
            return pGetResults();
        }

        DOEMeanStdDev pGetResults()
        {
            return results;
        }

        public DOECell(int col, int row)
        {
            pDOECell(col, row);
        }

        void pDOECell(int col, int row)
        {
            colIndex = col;
            rowIndex = row;
            results = new DOEMeanStdDev();
        }

        public void AddResultToCell(string result)
        {
            pAddResultToCell(result);
        }

        void pAddResultToCell(string result)
        {
            results.AddResult(result);
        }
    }

    public class DOEMeanStdDev
    {
        List<string> values;

        public List<string> getValues()
        {
            return pGetValues();
        }

        List<string> pGetValues()
        {
            return values;
        }

        public DOEMeanStdDev()
        {
            pDOEMeanStdDev();
        }

        void pDOEMeanStdDev()
        {
            values = new List<string>();
        }

        public void AddResult(string result)
        {
            pAddResult(result);
        }

        void pAddResult(string result)
        {
            values.Add(result);
        }
    }
}
