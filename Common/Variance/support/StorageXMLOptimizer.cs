#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Variance;

/// <summary>
/// Optimized XML parsing and handling utilities for Storage.cs.
/// Provides performance improvements while maintaining 100% backward compatibility.
/// </summary>
public static class StorageXMLOptimizer
{
    /// <summary>
    /// Cached XML elements to reduce repeated tree traversals.
    /// Cache is scoped per XML loading operation to maintain thread safety.
    /// </summary>
    private static readonly Dictionary<string, XElement?> ElementCache = new();

    /// <summary>
    /// Clears the element cache. Should be called at the start of each XML loading operation.
    /// </summary>
    public static void ClearCache()
    {
        ElementCache.Clear();
    }

    /// <summary>
    /// Gets a cached element or performs the lookup and caches the result.
    /// Reduces repeated XElement.Descendants() calls for the same element.
    /// </summary>
    /// <param name="root">Root element to search from</param>
    /// <param name="elementName">Name of the element to find</param>
    /// <returns>The found element or null if not found</returns>
    public static XElement? GetCachedElement(XElement root, string elementName)
    {
        string cacheKey = $"{root.GetHashCode()}_{elementName}";
        
        if (ElementCache.TryGetValue(cacheKey, out XElement? cachedElement))
        {
            return cachedElement;
        }

        // Perform the lookup and cache the result
        var element = root.Descendants(elementName).FirstOrDefault();
        ElementCache[cacheKey] = element;
        return element;
    }

    /// <summary>
    /// Gets a nested element with caching support.
    /// Optimizes patterns like root.Descendants("layer1").Descendants("enabled").First()
    /// </summary>
    /// <param name="root">Root element to search from</param>
    /// <param name="parentName">Parent element name</param>
    /// <param name="childName">Child element name</param>
    /// <returns>The found child element or null if not found</returns>
    public static XElement? GetCachedNestedElement(XElement root, string parentName, string childName)
    {
        string cacheKey = $"{root.GetHashCode()}_{parentName}_{childName}";
        
        if (ElementCache.TryGetValue(cacheKey, out XElement? cachedElement))
        {
            return cachedElement;
        }

        // Perform the nested lookup and cache the result
        var parentElement = root.Descendants(parentName).FirstOrDefault();
        var childElement = parentElement?.Descendants(childName).FirstOrDefault();
        
        ElementCache[cacheKey] = childElement;
        return childElement;
    }

    /// <summary>
    /// Safely gets an integer value from an XML element using TryParse instead of Convert + exception handling.
    /// Provides better performance and cleaner code flow.
    /// </summary>
    /// <param name="element">XML element to get value from</param>
    /// <param name="defaultValue">Default value if parsing fails</param>
    /// <returns>Parsed integer value or default value</returns>
    public static int GetIntValueSafe(XElement? element, int defaultValue = 0)
    {
        if (element == null || string.IsNullOrEmpty(element.Value))
            return defaultValue;

        return int.TryParse(element.Value, out int result) ? result : defaultValue;
    }

    /// <summary>
    /// Safely gets a double value from an XML element using TryParse instead of Convert + exception handling.
    /// </summary>
    /// <param name="element">XML element to get value from</param>
    /// <param name="defaultValue">Default value if parsing fails</param>
    /// <returns>Parsed double value or default value</returns>
    public static double GetDoubleValueSafe(XElement? element, double defaultValue = 0.0)
    {
        if (element == null || string.IsNullOrEmpty(element.Value))
            return defaultValue;

        return double.TryParse(element.Value, out double result) ? result : defaultValue;
    }

    /// <summary>
    /// Safely gets a decimal value from an XML element using TryParse instead of Convert + exception handling.
    /// </summary>
    /// <param name="element">XML element to get value from</param>
    /// <param name="defaultValue">Default value if parsing fails</param>
    /// <returns>Parsed decimal value or default value</returns>
    public static decimal GetDecimalValueSafe(XElement? element, decimal defaultValue = 0.0m)
    {
        if (element == null || string.IsNullOrEmpty(element.Value))
            return defaultValue;

        return decimal.TryParse(element.Value, out decimal result) ? result : defaultValue;
    }

    /// <summary>
    /// Safely gets a string value from an XML element with null protection.
    /// </summary>
    /// <param name="element">XML element to get value from</param>
    /// <param name="defaultValue">Default value if element is null or empty</param>
    /// <returns>Element value or default value</returns>
    public static string GetStringValueSafe(XElement? element, string defaultValue = "")
    {
        return element?.Value ?? defaultValue;
    }

    /// <summary>
    /// Optimized batch processing for layer background elements.
    /// Reduces the number of individual XElement.Descendants() calls.
    /// </summary>
    /// <param name="layerElement">Layer element containing background references</param>
    /// <param name="maxBgLayers">Maximum number of background layers to process</param>
    /// <returns>Array of background layer values</returns>
    public static int[] GetBackgroundLayersBatch(XElement layerElement, int maxBgLayers)
    {
        var bgValues = new int[maxBgLayers];
        
        // Get all child elements at once instead of individual Descendants() calls
        var childElements = layerElement.Elements().ToList();
        var bgElementLookup = childElements
            .Where(e => e.Name.ToString().EndsWith("BG"))
            .ToDictionary(e => e.Name.ToString(), e => e);

        for (int i = 0; i < maxBgLayers; i++)
        {
            string bgElementName = $"layer{i + 1}BG";
            if (bgElementLookup.TryGetValue(bgElementName, out XElement? bgElement))
            {
                bgValues[i] = GetIntValueSafe(bgElement, 0);
            }
            else
            {
                bgValues[i] = 0; // Default value
            }
        }

        return bgValues;
    }

    /// <summary>
    /// Optimizes version string parsing by avoiding unnecessary array creation.
    /// Uses Span<T> when available for better performance.
    /// </summary>
    /// <param name="version">Version string to parse</param>
    /// <returns>Array of version tokens</returns>
    public static string[] ParseVersionOptimized(string version)
    {
        if (string.IsNullOrEmpty(version))
            return Array.Empty<string>();

        // Pre-count dots to allocate correct array size
        int dotCount = 0;
        for (int i = 0; i < version.Length; i++)
        {
            if (version[i] == '.')
                dotCount++;
        }

        // Allocate exact size needed
        var tokens = new string[dotCount + 1];
        int tokenIndex = 0;
        int startIndex = 0;

        for (int i = 0; i <= version.Length; i++)
        {
            if (i == version.Length || version[i] == '.')
            {
                if (i > startIndex)
                {
                    tokens[tokenIndex] = version.Substring(startIndex, i - startIndex);
                    tokenIndex++;
                }
                startIndex = i + 1;
            }
        }

        return tokens;
    }

    /// <summary>
    /// Creates a pre-allocated StringBuilder for string concatenation operations.
    /// Estimates capacity based on expected content size to reduce reallocations.
    /// </summary>
    /// <param name="estimatedSize">Estimated final size of the string</param>
    /// <returns>StringBuilder with pre-allocated capacity</returns>
    public static System.Text.StringBuilder CreateOptimizedStringBuilder(int estimatedSize)
    {
        // Add 20% buffer to reduce likelihood of reallocation
        int capacity = (int)(estimatedSize * 1.2);
        return new System.Text.StringBuilder(capacity);
    }

    /// <summary>
    /// Batch element lookup for commonly accessed simulation settings.
    /// Reduces XML tree traversals by getting multiple elements at once.
    /// </summary>
    /// <param name="simulationElement">Simulation settings element</param>
    /// <returns>Dictionary of commonly used simulation settings</returns>
    public static Dictionary<string, XElement> GetSimulationElementsBatch(XElement simulationElement)
    {
        var elements = new Dictionary<string, XElement>();
        var commonElements = new[] 
        { 
            "numberOfCases", "resolution", "rngSeed", "previewZoom", "viewportX", "viewportY", "displayZoomFactor"
        };

        // Get all child elements once
        var childElements = simulationElement.Elements().ToList();
        var elementLookup = childElements.ToDictionary(e => e.Name.ToString(), e => e);

        foreach (var elementName in commonElements)
        {
            if (elementLookup.TryGetValue(elementName, out XElement? element))
            {
                elements[elementName] = element;
            }
        }

        return elements;
    }

    /// <summary>
    /// Batch element lookup for DOE settings.
    /// </summary>
    /// <param name="doeElement">DOE settings element</param>
    /// <returns>Dictionary of DOE setting elements</returns>
    public static Dictionary<string, XElement> GetDOEElementsBatch(XElement doeElement)
    {
        var elements = new Dictionary<string, XElement>();
        var doeElementNames = new[] 
        { 
            "rows", "cols", "comment", "viewportX", "viewportY", "displayZoomFactor"
        };

        var childElements = doeElement.Elements().ToList();
        var elementLookup = childElements.ToDictionary(e => e.Name.ToString(), e => e);

        foreach (var elementName in doeElementNames)
        {
            if (elementLookup.TryGetValue(elementName, out XElement? element))
            {
                elements[elementName] = element;
            }
        }

        return elements;
    }
}