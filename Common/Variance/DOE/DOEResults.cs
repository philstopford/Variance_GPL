using System.Collections.Generic;
using System.Linq;

namespace Variance;

public class DOEResults
{
    private List<DOEResult> results;

    public List<DOEResult> getResults()
    {
        return pGetResults();
    }

    private List<DOEResult> pGetResults()
    {
        return results;
    }

    public DOEResults(string DOEName)
    {
        pDOEResults(DOEName);
    }

    private void pDOEResults(string DOEName)
    {
        results = new List<DOEResult> {new(DOEName)};
    }

    public void AddResult(string DOEName)
    {
        pAddResult(DOEName);
    }

    private void pAddResult(string DOEName)
    {
        results.Add(new DOEResult(DOEName));
    }
}

public class DOEResult
{
    private string DOEName;

    private bool headerSet;

    private List<string> doeHeaderInformation;

    private List<DOECell> cells;

    public List<DOECell> getCells()
    {
        return pGetCells();
    }

    private List<DOECell> pGetCells()
    {
        return cells;
    }

    public List<string> getHeaderInformation()
    {
        return pGetHeaderInformation();
    }

    private List<string> pGetHeaderInformation()
    {
        return doeHeaderInformation;
    }

    public string getName()
    {
        return pGetName();
    }

    private string pGetName()
    {
        return DOEName;
    }

    public DOEResult(string DOEName_)
    {
        pDOEResult(DOEName_);
    }

    private void pDOEResult(string DOEName_)
    {
        DOEName = DOEName_;
        headerSet = false;
        cells = new List<DOECell>();
        doeHeaderInformation = new List<string> {""};
    }

    public void setDOEHeaderInformation(List<string> headerInformation)
    {
        pSetDOEHeaderInformation(headerInformation);
    }

    private void pSetDOEHeaderInformation(List<string> headerInformation)
    {
        if (headerSet)
        {
            return;
        }

        doeHeaderInformation = headerInformation.ToList();
        headerSet = true;
    }

    public void AddCellToDOE(int col, int row)
    {
        pAddCellToDOE(col, row);
    }

    private void pAddCellToDOE(int col, int row)
    {
        cells.Add(new DOECell(col, row));
    }

    public void AddResultToCell(int col, int row, string result)
    {
        pAddResultToCell(col, row, result);
    }

    private void pAddResultToCell(int col, int row, string result)
    {
        foreach (DOECell t in cells.Where(t => t.getRowIndex() == row && t.getColIndex() == col))
        {
            t.AddResultToCell(result);
        }
    }
}

public class DOECell
{
    private int colIndex;
    private int rowIndex;
    private DOEMeanStdDev results;

    public int getColIndex()
    {
        return pGetColIndex();
    }

    private int pGetColIndex()
    {
        return colIndex;
    }

    public int getRowIndex()
    {
        return pGetRowIndex();
    }

    private int pGetRowIndex()
    {
        return rowIndex;
    }

    public DOEMeanStdDev getResults()
    {
        return pGetResults();
    }

    private DOEMeanStdDev pGetResults()
    {
        return results;
    }

    public DOECell(int col, int row)
    {
        pDOECell(col, row);
    }

    private void pDOECell(int col, int row)
    {
        colIndex = col;
        rowIndex = row;
        results = new DOEMeanStdDev();
    }

    public void AddResultToCell(string result)
    {
        pAddResultToCell(result);
    }

    private void pAddResultToCell(string result)
    {
        results.AddResult(result);
    }
}

public class DOEMeanStdDev
{
    private List<string> values;

    public List<string> getValues()
    {
        return pGetValues();
    }

    private List<string> pGetValues()
    {
        return values;
    }

    public DOEMeanStdDev()
    {
        pDOEMeanStdDev();
    }

    private void pDOEMeanStdDev()
    {
        values = new List<string>();
    }

    public void AddResult(string result)
    {
        pAddResult(result);
    }

    private void pAddResult(string result)
    {
        values.Add(result);
    }
}