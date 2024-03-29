﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Variance;

[Serializable]
public class DOESettings
{
    public static string default_comment = "";
    public static double default_colOffset = 0;
    public static double default_rowOffset = 0;
    public static int[] default_layersAffected = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static double default_colPitch = 0;
    public static double default_rowPitch = 0;
    public static int default_cols = 1;
    public static int default_rows = 1;
    public static int default_specificTile = 0;
    public static int default_specificTile_Row = 0;
    public static int default_specificTile_Col = 0;
    public static int default_listOfTiles = 0;
    public static List<int[]> default_tileList_ColRow = new() { new [] { 0, 0 } };
    public static string default_trList = "";

    public enum properties_d { colOffset, rowOffset, colPitch, rowPitch }

    private double colOffset;
    private double rowOffset;
    private double rowPitch;
    private double colPitch;
    public double getDouble(properties_d p)
    {
        return pGetDouble(p);
    }

    private double pGetDouble(properties_d p)
    {
        double ret = 0;

        switch (p)
        {
            case properties_d.colOffset:
                ret = colOffset;
                break;
            case properties_d.rowOffset:
                ret = rowOffset;
                break;
            case properties_d.colPitch:
                ret = colPitch;
                break;
            case properties_d.rowPitch:
                ret = rowPitch;
                break;
        }

        return ret;
    }

    public void setDouble(properties_d p, double val)
    {
        pSetDouble(p, val);
    }

    private void pSetDouble(properties_d p, double val)
    {
        switch (p)
        {
            case properties_d.colOffset:
                colOffset = val;
                break;
            case properties_d.rowOffset:
                rowOffset = val;
                break;
            case properties_d.colPitch:
                colPitch = val;
                break;
            case properties_d.rowPitch:
                rowPitch = val;
                break;
        }
    }

    public enum properties_i { rows, cols, sTile, sTileRow, sTileCol, uTileList }

    private int rows;
    private int cols;
    private int specificTile;
    private int specificTile_Row;
    private int specificTile_Col;
    private int listOfTiles; // 0 is inactive, 1 is active.

    public int getInt(properties_i p)
    {
        return pGetInt(p);
    }

    private int pGetInt(properties_i p)
    {
        int ret = 0;

        switch (p)
        {
            case properties_i.cols:
                ret = cols;
                break;
            case properties_i.rows:
                ret = rows;
                break;
            case properties_i.sTile:
                ret = specificTile;
                break;
            case properties_i.sTileCol:
                ret = specificTile_Col;
                break;
            case properties_i.sTileRow:
                ret = specificTile_Row;
                break;
            case properties_i.uTileList:
                ret = listOfTiles;
                break;
        }

        return ret;
    }

    public void setInt(properties_i p, int val)
    {
        pSetInt(p, val);
    }

    private void pSetInt(properties_i p, int val)
    {
        switch (p)
        {
            case properties_i.cols:
                cols = val;
                break;
            case properties_i.rows:
                rows = val;
                break;
            case properties_i.sTile:
                specificTile = val;
                break;
            case properties_i.sTileCol:
                specificTile_Col = val;
                break;
            case properties_i.sTileRow:
                specificTile_Row = val;
                break;
            case properties_i.uTileList:
                listOfTiles = val;
                break;
        }
    }

    private int[] layersAffected;

    public int[] getLayersAffected()
    {
        return pGetLayersAffected();
    }

    private int[] pGetLayersAffected()
    {
        return layersAffected;
    }

    public void setLayersAffected(List<int[]> val)
    {
        pSetLayersAffected(val);
    }

    private void pSetLayersAffected(List<int[]> val)
    {
        tileList_ColRow = val.ToList();
    }

    public int getLayerAffected(int layer)
    {
        return pGetLayerAffected(layer);
    }

    private int pGetLayerAffected(int layer)
    {
        return layersAffected[layer];
    }

    public void setLayerAffected(int layer, int val)
    {
        pSetLayerAffected(layer, val);
    }

    private void pSetLayerAffected(int layer, int val)
    {
        layersAffected[layer] = val;
    }

    private List<int[]> tileList_ColRow; // 2 fields : col, row

    public void resetTileList_ColRow()
    {
        pResetTileList_ColRow();
    }

    private void pResetTileList_ColRow()
    {
        tileList_ColRow = new List<int[]>();
    }

    public List<int[]> getTileList_ColRow()
    {
        return pGetTileList_ColRow();
    }

    private List<int[]> pGetTileList_ColRow()
    {
        return tileList_ColRow;
    }

    public int getTileList_Value(int list, int index)
    {
        return pGetTileList_Value(list, index);
    }

    private int pGetTileList_Value(int list, int index)
    {
        return tileList_ColRow[list][index];
    }

    public void setTileList_Value(int list, int index, int val)
    {
        pSetTileList_ColRow(list, index, val);
    }

    private void pSetTileList_ColRow(int list, int index, int val)
    {
        tileList_ColRow[list][index] = val;
    }

    public void setTileList_ColRow(List<int[]> val)
    {
        pSetTileList_ColRow(val);
    }

    private void pSetTileList_ColRow(List<int[]> val)
    {
        tileList_ColRow = val.ToList();
    }

    public void addTileList_Value(int[] val)
    {
        pAddTileList_Value(val);
    }

    private void pAddTileList_Value(int[] val)
    {
        tileList_ColRow.Add(val);
    }

    public enum properties_b { iDRM, quilt }

    private bool iDRMRunConfigured;
    private bool OPCRunConfigured;

    public bool getBool(properties_b p)
    {
        return pGetBool(p);
    }

    private bool pGetBool(properties_b p)
    {
        bool ret = false;
        switch (p)
        {
            case properties_b.iDRM:
                ret = iDRMRunConfigured;
                break;
            case properties_b.quilt:
                ret = OPCRunConfigured;
                break;
        }
        return ret;
    }

    public void setBool(properties_b p, bool val)
    {
        pSetBool(p, val);
    }

    private void pSetBool(properties_b p, bool val)
    {
        switch (p)
        {
            case properties_b.iDRM:
                iDRMRunConfigured = val;
                break;
            case properties_b.quilt:
                OPCRunConfigured = val;
                break;
        }
    }

    public enum properties_s { comment, list }

    private string comment;
    private string trList;

    public string getString(properties_s p)
    {
        return pGetString(p);
    }

    private string pGetString(properties_s p)
    {
        string ret = "";
        switch (p)
        {
            case properties_s.comment:
                ret = comment;
                break;
            case properties_s.list:
                ret = trList;
                break;
        }
        return ret;
    }

    public void setString(properties_s p, string val)
    {
        pSetString(p, val);
    }

    private void pSetString(properties_s p, string val)
    {
        switch (p)
        {
            case properties_s.comment:
                comment = val;
                break;
            case properties_s.list:
                trList = val;
                break;
        }
    }

    public DOESettings()
    {
        pDOESettings();
    }

    private void pDOESettings()
    {
        comment = default_comment;
        colOffset = default_colOffset;
        rowOffset = default_rowOffset;
        layersAffected = default_layersAffected;
        rowPitch = default_rowPitch;
        colPitch = default_colPitch;
        rows = default_rows;
        cols = default_cols;
        specificTile = default_specificTile;
        specificTile_Col = default_specificTile_Col;
        specificTile_Row = default_specificTile_Row;
        listOfTiles = default_listOfTiles;
        tileList_ColRow = default_tileList_ColRow;
        iDRMRunConfigured = false;
        OPCRunConfigured = false;
        trList = default_trList; // used for loading, as a container.
    }

    public DOESettings(int rP, int cP, int r, int c)
    {
        pDOESettings(rP, cP, r, c);
    }

    private void pDOESettings(int rP, int cP, int r, int c)
    {
        comment = default_comment;
        colOffset = default_colOffset;
        rowOffset = default_rowOffset;
        layersAffected = default_layersAffected;
        rowPitch = rP;
        colPitch = cP;
        rows = r;
        cols = c;
        specificTile = default_specificTile;
        specificTile_Col = default_specificTile_Col;
        specificTile_Row = default_specificTile_Row;
        listOfTiles = default_listOfTiles;
        tileList_ColRow = default_tileList_ColRow;
        iDRMRunConfigured = false;
        OPCRunConfigured = false;
        trList = default_trList; // used for loading, as a container.
    }
}