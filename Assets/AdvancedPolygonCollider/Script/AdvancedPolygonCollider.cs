/*
Advanced Polygon Collider (c) 2015 Digital Ruby, LLC
http://www.digitalruby.com

Source code may not be redistributed. Use in apps and games is fine.
*/

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace DigitalRuby.AdvancedPolygonCollider
{
    public struct PolygonParameters
    {
        /// <summary>
        /// Texture - must be readable and writeable.
        /// </summary>
        public Texture2D Texture;

        /// <summary>
        /// Source rect from the texture containing the sprite.
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// Offset (pivot) for the sprite
        /// </summary>
        public Vector2 Offset;

        /// <summary>
        /// X multiplier if pixels per unit are used, otherwise 1 (see UpdatePolygonCollider method).
        /// </summary>
        public float XMultiplier;

        /// <summary>
        /// Y multiplier if pixels per unit are used, otherwise 1 (see UpdatePolygonCollider method).
        /// </summary>
        public float YMultiplier;

        /// <summary>
        /// Alpha tolerance. Pixels with greater than this are considered solid.
        /// </summary>
        public byte AlphaTolerance;

        /// <summary>
        /// Distance threshold to collapse vertices in pixels.
        /// </summary>
        public int DistanceThreshold;

        /// <summary>
        /// True to decompose into convex polygons, false otherwise.
        /// </summary>
        public bool Decompose;

        /// <summary>
        /// Whether to use the cache. Values will be cached accordingly.
        /// </summary>
        public bool UseCache;

        public override int GetHashCode()
        {
            int h = Texture.GetHashCode();
            if (h == 0)
            {
                h = 1;
            }
            return h * (int)(Rect.GetHashCode() * XMultiplier * YMultiplier * AlphaTolerance * Mathf.Max(DistanceThreshold, 1) * (Decompose ? 2 : 1));
        }

        public override bool Equals(object obj)
        {
            if (obj is PolygonParameters)
            {
                PolygonParameters p = (PolygonParameters)obj;
                return Texture == p.Texture && Rect == p.Rect &&
                    XMultiplier == p.XMultiplier && YMultiplier == p.YMultiplier &&
                    AlphaTolerance == p.AlphaTolerance && DistanceThreshold == p.DistanceThreshold &&
                    Decompose == p.Decompose;
            }
            return false;
        }
    }

    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteInEditMode]
    public class AdvancedPolygonCollider : MonoBehaviour
    {
        [Serializable]
        public struct ArrayWrapper
        {
            [SerializeField]
            public Vector2[] Array;
        }

        [Serializable]
        public struct ListWrapper
        {
            [SerializeField]
            public List<ArrayWrapper> List;
        }

        [Serializable]
        public struct CacheEntry
        {
            [SerializeField]
            public CacheKey Key;

            [SerializeField]
            public ListWrapper Value;
        }

        [Serializable]
        public struct CacheKey
        {
            [SerializeField]
            public Texture2D Texture;

            [SerializeField]
            public Rect Rect;

            public override int GetHashCode()
            {
                return Texture.GetHashCode() * Rect.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is CacheKey)
                {
                    CacheKey k = (CacheKey)obj;
                    return (Texture == k.Texture && Rect == k.Rect);
                }
                return false;
            }
        }

        [Tooltip("Pixels with alpha greater than this count as solid.")]
        [Range(0, 254)]
        public byte AlphaTolerance = 20;

        [Tooltip("Points further away than this number of pixels will be consolidated.")]
        [Range(0, 64)]
        public int DistanceThreshold = 8;

        [Tooltip("Scale of the polygon.")]
        [Range(0.5f, 2.0f)]
        public float Scale = 1.0f;

        [Tooltip("Whether to decompse vertices into convex only polygons.")]
        public bool Decompose = false;

        [Tooltip("Whether to live update everything when in play mode. Typically for performance this can be false, " +
            "but if you plan on making changes to the sprite or parameters at runtime, you will want to set this to true.")]
        public bool RunInPlayMode = false;

        [Tooltip("True to use the cache, false otherwise. The cache is populated in editor and play mode and uses the most recent geometry " +
            "for a texture and rect regardless of other parameters. When ignoring the cache, values will not be added to the cache either. Cache is " +
            "only useful if you will be changing your sprite at run-time (i.e. animation)")]
        public bool UseCache;

        private SpriteRenderer spriteRenderer;
        private PolygonCollider2D polygonCollider;
        private bool dirty;

        [SerializeField]
        [HideInInspector]
        private byte lastAlphaTolerance;

        [SerializeField]
        [HideInInspector]
        private float lastScale;

        [SerializeField]
        [HideInInspector]
        private int lastDistanceThreshold;

        [SerializeField]
        [HideInInspector]
        private bool lastDecompose;

        [SerializeField]
        [HideInInspector]
        private Sprite lastSprite;

        [SerializeField]
        [HideInInspector]
        private Rect lastRect = new Rect();

        [SerializeField]
        [HideInInspector]
        private Vector2 lastOffset = new Vector2(-99999.0f, -99999.0f);

        [SerializeField]
        [HideInInspector]
        private float lastPixelsPerUnit;

        [SerializeField]
        [HideInInspector]
        private bool lastFlipX;

        [SerializeField]
        [HideInInspector]
        private bool lastFlipY;

        private static readonly Dictionary<CacheKey, List<Vector2[]>> cache = new Dictionary<CacheKey, List<Vector2[]>>();

        [Tooltip("All the cached objects from the editor. Do not modify this data.")]
        [SerializeField]
        private List<CacheEntry> editorCache = new List<CacheEntry>();

        // private readonly AdvancedPolygonColliderAutoGeometry geometryDetector = new AdvancedPolygonColliderAutoGeometry();
        private readonly TextureConverter geometryDetector = new TextureConverter();

#if UNITY_EDITOR

        private Texture2D blackBackground;

#endif

        private void Awake()
        {
            if (Application.isPlaying)
            {
                // move editor cache to regular cache
                foreach (var v in editorCache)
                {
                    List<Vector2[]> list = new List<Vector2[]>();
                    cache[v.Key] = list;
                    foreach (ArrayWrapper w in v.Value.List)
                    {
                        list.Add(w.Array);
                    }
                }
            }
        }

        private void Start()
        {

#if UNITY_EDITOR

            blackBackground = new Texture2D(1, 1);
            blackBackground.SetPixel(0, 0, new Color(0.0f, 0.0f, 0.0f, 0.8f));
            blackBackground.Apply();

#endif

            polygonCollider = GetComponent<PolygonCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void UpdateDirtyState()
        {
            if (spriteRenderer.sprite != lastSprite)
            {
                lastSprite = spriteRenderer.sprite;
                dirty = true;
            }
            if (spriteRenderer.sprite != null)
            {
                if (lastOffset != spriteRenderer.sprite.pivot)
                {
                    lastOffset = spriteRenderer.sprite.pivot;
                    dirty = true;
                }
                if (lastRect != spriteRenderer.sprite.rect)
                {
                    lastRect = spriteRenderer.sprite.rect;
                    dirty = true;
                }
                if (lastPixelsPerUnit != spriteRenderer.sprite.pixelsPerUnit)
                {
                    lastPixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
                    dirty = true;
                }
                if (lastFlipX != spriteRenderer.flipX)
                {
                    lastFlipX = spriteRenderer.flipX;
                    dirty = true;
                }
                if (lastFlipY != spriteRenderer.flipY)
                {
                    lastFlipY = spriteRenderer.flipY;
                    dirty = true;
                }
            }
            if (AlphaTolerance != lastAlphaTolerance)
            {
                lastAlphaTolerance = AlphaTolerance;
                dirty = true;
            }
            if (Scale != lastScale)
            {
                lastScale = Scale;
                dirty = true;
            }
            if (DistanceThreshold != lastDistanceThreshold)
            {
                lastDistanceThreshold = DistanceThreshold;
                dirty = true;
            }
            if (Decompose != lastDecompose)
            {
                lastDecompose = Decompose;
                dirty = true;
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (!RunInPlayMode)
                {
                    return;
                }
            }
            else if (!UseCache)
            {
                editorCache.Clear();
            }

            UpdateDirtyState();
            if (dirty)
            {
                RecalculatePolygon();
            }
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.BeginGUI();
            GUI.color = Color.white;
            string text = " Vertices: " + VerticesCount + " ";
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(gameObject.transform.position);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.skin.box.normal.background = blackBackground;
            Rect rect = new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y);
            GUI.Box(rect, GUIContent.none);
            GUI.Label(rect, text);
            UnityEditor.Handles.EndGUI();
        }

        private void AddEditorCache(ref PolygonParameters p, List<Vector2[]> list)
        {
            CacheKey key = new CacheKey();
            key.Texture = p.Texture;
            key.Rect = p.Rect;

            CacheEntry e = new CacheEntry();
            e.Key = key;
            e.Value = new ListWrapper();
            e.Value.List = new List<ArrayWrapper>();
            foreach (Vector2[] v in list)
            {
                ArrayWrapper w = new ArrayWrapper();
                w.Array = v;
                e.Value.List.Add(w);
            }

            for (int i = 0; i < editorCache.Count; i++)
            {
                if (editorCache[i].Key.Equals(key))
                {
                    editorCache.RemoveAt(i);
                    editorCache.Insert(i, e);
                    return;
                }
            }

            editorCache.Add(e);
        }

#endif

        public void RecalculatePolygon()
        {
            if (spriteRenderer.sprite != null)
            {
                PolygonParameters p = new PolygonParameters();
                p.AlphaTolerance = AlphaTolerance;
                p.Decompose = Decompose;
                p.DistanceThreshold = DistanceThreshold;
                p.Rect = spriteRenderer.sprite.rect;
                p.Offset = spriteRenderer.sprite.pivot;
                p.Texture = spriteRenderer.sprite.texture;
                p.XMultiplier = (spriteRenderer.sprite.rect.width * 0.5f) / spriteRenderer.sprite.pixelsPerUnit;
                p.YMultiplier = (spriteRenderer.sprite.rect.height * 0.5f) / spriteRenderer.sprite.pixelsPerUnit;
                p.UseCache = UseCache;
                UpdatePolygonCollider(ref p);
            }
        }

        public void UpdatePolygonCollider(ref PolygonParameters p)
        {
            if (spriteRenderer.sprite == null || spriteRenderer.sprite.texture == null)
            {
                return;
            }

            dirty = false;
            List<Vector2[]> cached;

            if (Application.isPlaying && p.UseCache)
            {
                CacheKey key = new CacheKey();
                key.Texture = p.Texture;
                key.Rect = p.Rect;

                if (cache.TryGetValue(key, out cached))
                {
                    polygonCollider.pathCount = cached.Count;
                    for (int i = 0; i < cached.Count; i++)
                    {
                        polygonCollider.SetPath(i, cached[i]);
                    }

                    return;
                }
            }

            PopulateCollider(polygonCollider, ref p);
        }

        public int VerticesCount
        {
            get { return (polygonCollider == null ? 0 : polygonCollider.GetTotalPointCount()); }
        }

        /// <summary>
        /// Populate the vertices of a collider
        /// </summary>
        /// <param name="collider">Collider to setup vertices in.</param>
        /// <param name="p">Polygon creation parameters</param>
        public void PopulateCollider(PolygonCollider2D collider, ref PolygonParameters p)
        {
            try
            {
                if (p.Texture.format != TextureFormat.ARGB32 && p.Texture.format != TextureFormat.BGRA32 && p.Texture.format != TextureFormat.RGBA32 &&
                    p.Texture.format != TextureFormat.RGB24 && p.Texture.format != TextureFormat.Alpha8 && p.Texture.format != TextureFormat.RGBAFloat &&
                    p.Texture.format != TextureFormat.RGBAHalf && p.Texture.format != TextureFormat.RGB565)
                {
                    Debug.LogWarning("Advanced Polygon Collider works best with a non-compressed texture in ARGB32, BGRA32, RGB24, RGBA4444, RGB565, RGBAFloat or RGBAHalf format");
                }
                int width = (int)p.Rect.width;
                int height = (int)p.Rect.height;
                int x = (int)p.Rect.x;
                int y = (int)p.Rect.y;
                UnityEngine.Color[] pixels = p.Texture.GetPixels(x, y, width, height, 0);
                List<Vertices> verts = geometryDetector.DetectVertices(pixels, width, p.AlphaTolerance);
                int pathIndex = 0;
                List<Vector2[]> list = new List<Vector2[]>();

                for (int i = 0; i < verts.Count; i++)
                {
                    ProcessVertices(collider, verts[i], list, ref p, ref pathIndex);
                }

#if UNITY_EDITOR

                if (Application.isPlaying)
                {

#endif

                    if (p.UseCache)
                    {
                        CacheKey key = new CacheKey();
                        key.Texture = p.Texture;
                        key.Rect = p.Rect;
                        cache[key] = list;
                    }

#if UNITY_EDITOR

                }
                else if (p.UseCache)
                {
                    AddEditorCache(ref p, list);
                }

#endif

                Debug.Log("Updated polygon.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error creating collider: " + ex);
            }
        }

        private List<Vector2[]> ProcessVertices(PolygonCollider2D collider, Vertices v, List<Vector2[]> list, ref PolygonParameters p, ref int pathIndex)
        {
            Vector2 offset = p.Offset;
            float flipXMultiplier = (spriteRenderer.flipX ? -1.0f : 1.0f);
            float flipYMultiplier = (spriteRenderer.flipY ? -1.0f : 1.0f);

			if (p.DistanceThreshold > 1)
			{
				v = SimplifyTools.DouglasPeuckerSimplify (v, p.DistanceThreshold);
			}

            if (p.Decompose)
            {
                List<List<Vector2>> points = BayazitDecomposer.ConvexPartition(v);
                for (int j = 0; j < points.Count; j++)
                {
                    List<Vector2> v2 = points[j];
                    for (int i = 0; i < v2.Count; i++)
                    {
						float xValue = (2.0f * (((v2[i].x - offset.x) + 0.5f) / p.Rect.width));
						float yValue = (2.0f * (((v2[i].y - offset.y) + 0.5f) / p.Rect.height));
                        v2[i] = new Vector2(xValue * p.XMultiplier * Scale * flipXMultiplier, yValue * p.YMultiplier * Scale * flipYMultiplier);
                    }
                    Vector2[] arr = v2.ToArray();
                    collider.pathCount = pathIndex + 1;
                    collider.SetPath(pathIndex++, arr);
                    list.Add(arr);
                }
            }
            else
            {
                collider.pathCount = pathIndex + 1;
                for (int i = 0; i < v.Count; i++)
                {
					float xValue = (2.0f * (((v[i].x - offset.x) + 0.5f) / p.Rect.width));
					float yValue = (2.0f * (((v[i].y - offset.y) + 0.5f) / p.Rect.height));
                    v[i] = new Vector2(xValue * p.XMultiplier * Scale * flipXMultiplier, yValue * p.YMultiplier * Scale * flipYMultiplier);
                }
                Vector2[] arr = v.ToArray();
                collider.SetPath(pathIndex++, arr);
                list.Add(arr);
            }

            return list;
        }
    }
}