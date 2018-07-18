using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerrain
{

    #region parameter goals for the game
    // RENDER_DISTANCE = 16
    // WIDTH and DEPTH = 40
    // HEIGHT = 16
    #endregion

    //don't think this has to be odd because its more of an offset from center
    public const int RENDER_DISTANCE = 3;
    public const int CENTERED_WIDTH = (2 * RENDER_DISTANCE) + 1;
    public const int CENTERED_HEIGHT = (2 * RENDER_DISTANCE) + 1;
    public const int CENTERED_DEPTH = (2 * RENDER_DISTANCE) + 1;
    
    //use odd numbers for best results
    public const int WIDTH =  5;   
    public const int DEPTH =  5;  
    public const int HEIGHT = 5;  

    //noise mang
    public const int NOISE_WIDTH = WIDTH * Chunk.WIDTH + 1;
    public const int NOISE_DEPTH = DEPTH * Chunk.DEPTH + 1;
    public const int NOISE_HEIGHT = HEIGHT * Chunk.HEIGHT + 1;

    //public static double[] noise = new double[NOISE_WIDTH * NOISE_HEIGHT * NOISE_DEPTH];
    public static double[,,] noise = new double[NOISE_WIDTH, NOISE_HEIGHT, NOISE_DEPTH];
    public static double isolevel = 0.5;
    private static double min = double.MaxValue;
    private static double max = double.MinValue;
    private static int normalizeValue = 1;

    private static Dictionary<Vector3, Chunk> allChunks = new Dictionary<Vector3, Chunk>();
    private static Dictionary<Vector3, Chunk> loadedChunks = new Dictionary<Vector3, Chunk>();
    private static Dictionary<Vector3, Chunk> unloadedChunks = new Dictionary<Vector3, Chunk>();
    private static Vector3 playerPosition;

    //debug stuff
    public bool showGizmos;

    #region INTERFACE
    public static void Generate()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        GenerateNoise();
    }
    public static void Update()
    {
        GenerateChunks();
        UnloadChunks();
        RenderChunks();
    }
    #endregion

    #region OVERRIDES
    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            for (int z = 0; z < 4; z++)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            Gizmos.color = Color.red;
                        else
                            Gizmos.color = Color.white;
                        Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
                    }
                }
            }
        }
    }
    #endregion

    #region HELPERS
    private static void GenerateNoise()
    {
        var simplexNoise = new OpenSimplexNoise();

        for (int z = 0; z < NOISE_DEPTH; z++)
        {
            for (int y = 0; y < NOISE_HEIGHT; y++)
            {
                for (int x = 0; x < NOISE_WIDTH; x++)
                {
                    #region basic example
                    //var octaves = 1;
                    //var deltaFrequency = 0.5;
                    //var deltaAmplitude = 2.0;
                    //var deltaScale = 1.0;

                    //var frequency = 1.0;
                    //var amplitude = 1.0;
                    //var scale = 1.0;

                    //double sample = 0.0;
                    //for (int i = 0; i < octaves; i++)
                    //{

                    //    sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
                    //                                ((double)y / (double)Chunk.HEIGHT) * frequency,
                    //                                ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 0.5;

                    //    frequency *= deltaFrequency;
                    //    amplitude *= deltaAmplitude;
                    //    scale *= deltaScale;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
                    #endregion

                    #region example with overhangs
                    //var octaves = 3;
                    //var deltaFrequency = 0.5;
                    //var deltaAmplitude = 2.0;
                    //var deltaScale = 1.0;

                    //var frequency = 2.0;
                    //var amplitude = 10.0;
                    //var scale = 5.0;

                    //double sample = 0.0;
                    //for (int i = 0; i < octaves; i++)
                    //{

                    //    sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
                    //                                ((double)y / (double)Chunk.HEIGHT) * frequency,
                    //                                ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 3f;

                    //    frequency *= deltaFrequency;
                    //    amplitude *= deltaAmplitude;
                    //    scale *= deltaScale;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    ////var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    ////noise[index] = sample;
                    //noise[x, y, z] = sample;

                    #endregion

                    #region giant cube example
                    double sample = 1.0;

                    if (x == 0 || x == NOISE_WIDTH - 1 || y == 0 || y == NOISE_HEIGHT - 1 || z == 0 || z == NOISE_DEPTH - 1)
                        sample = -1.0;

                    if (sample < min) min = sample;
                    if (sample > max) max = sample;

                    var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
                    noise[x, y, z] = sample;
                    #endregion

                    #region another 3D example
                    //var octaves = 3;
                    //var deltaFrequency = 0.5;
                    //var deltaAmplitude = 2.0;
                    //var deltaScale = 1.0;

                    //var frequency = 3.0;
                    //var amplitude = 10.0;
                    //var scale = 5.0;

                    //double sample = 0.0;
                    //for (int i = 0; i < octaves; i++)
                    //{

                    //    sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
                    //                                ((double)y / (double)Chunk.HEIGHT) * frequency,
                    //                                ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 3f;

                    //    frequency *= deltaFrequency;
                    //    amplitude *= deltaAmplitude;
                    //    scale *= deltaScale;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //noise[x, y, z] = sample;
                    #endregion  
                }
            }
        }

        double average = (min + max) / 2;
        double range = (max - min) / 2;

        for (int z = 0; z < NOISE_DEPTH; z++)
        {
            for (int y = 0; y < NOISE_HEIGHT; y++)
            {
                for (int x = 0; x < NOISE_WIDTH; x++)
                {
                    var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = (noise[index] - average) / range * normalizeValue;
                    noise[x, y, z] = (noise[x, y, z] - average) / range * normalizeValue;
                }
            }
        }
    }
    private static void GenerateChunks()
    {
        Spiral(CENTERED_WIDTH, CENTERED_HEIGHT, CENTERED_DEPTH);
    }
    private static void Spiral(int X, int Y, int Z)
    {
        bool flipped = false;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        int playerX = (int)(playerPosition.x / Chunk.WIDTH);
        if (playerPosition.x < 0) playerX--;
        int playerZ = (int)(playerPosition.z / Chunk.DEPTH);
        if (playerPosition.z < 0) playerZ--;
        int playerY = (int)playerPosition.y / Chunk.HEIGHT;

        int y = playerY;
        while (y > -Y + RENDER_DISTANCE)
        {
            int x, z, dx, dz;
            x = z = dx = 0;
            dz = -1;
            int maxI = X * Z;
            for (int i = 0; i < maxI; i++)
            {
                var normalizedPlayerPosition = new Vector3(playerX, playerY, playerZ);
                var localOffset = new Vector3(x, y, z);
                var newChunkPosition = GetNewChunkPosition(normalizedPlayerPosition, localOffset);
                var newChunkNoisePosition = GetNewChunkNoisePosition(normalizedPlayerPosition, localOffset);
                Chunk existingChunk; 

                //If the chunk doesn't exist and should, create it
                if (!ChunkExists(newChunkPosition, out existingChunk))
                {
                    if (NoiseExists(newChunkNoisePosition))
                    {
                        var go = new GameObject(newChunkPosition.ToString());
                        go.transform.position = newChunkPosition;

                        var chunk = new Chunk(go, newChunkNoisePosition);
                        allChunks.Add(go.transform.position, chunk);
                        loadedChunks.Add(go.transform.position, chunk);
                        chunk.GenerateTerrain();
                    }
                }
                //If the chunk does exist and isn't loaded, load it
                else
                {
                    Chunk tmpChunk;
                    if(!loadedChunks.TryGetValue(newChunkPosition, out tmpChunk))
                    {
                        unloadedChunks.TryGetValue(newChunkPosition, out tmpChunk);
                        tmpChunk.go.SetActive(true);
                        unloadedChunks.Remove(newChunkPosition);
                        loadedChunks.Add(newChunkPosition, tmpChunk);
                    }
                    
                }

                if ((x == z) ||
                    ((x < 0) && (x == -z)) ||
                    ((x > 0) && (x == 1 - z)))
                {
                    var t = dx;
                    dx = -dz;
                    dz = t;
                }

                //Mutate x,y,z at the end
                x += dx;
                z += dz;
            }

            if (y >= (Y + playerY) / 2 && !flipped)
            {
                y = playerY;
                flipped = true;
            }
            if (!flipped)
                y++;
            else
                y--;
        }

    }
    private static Vector3 GetNewChunkPosition(Vector3 normalizedPlayerPosition, Vector3 localOffset)
    {
        var chunkX = (localOffset.x + normalizedPlayerPosition.x) * Chunk.WIDTH;
        var chunkY = localOffset.y * Chunk.HEIGHT;
        if (normalizedPlayerPosition.y < 0) chunkY -= Chunk.HEIGHT;
        var chunkZ = (localOffset.z + normalizedPlayerPosition.z) * Chunk.DEPTH;

        return new Vector3(chunkX, chunkY, chunkZ);
    }
    private static Vector3 GetNewChunkNoisePosition(Vector3 normalizedPlayerPosition, Vector3 localOffset)
    {
        var noiseX = (localOffset.x + normalizedPlayerPosition.x + (NOISE_WIDTH - 1) / Chunk.WIDTH / 2) * Chunk.WIDTH;
        var noiseY = (localOffset.y + (NOISE_HEIGHT - 1) / Chunk.HEIGHT / 2) * Chunk.HEIGHT;
        if (normalizedPlayerPosition.y < 0) noiseY -= Chunk.HEIGHT;
        var noiseZ = (localOffset.z + normalizedPlayerPosition.z + (NOISE_DEPTH - 1) / Chunk.DEPTH / 2) * Chunk.DEPTH;

        return new Vector3(noiseX, noiseY, noiseZ);
    }
    private static bool ChunkExists(Vector3 position, out Chunk existingChunk)
    {
        return allChunks.TryGetValue(position, out existingChunk);
    }
    private static bool NoiseExists(Vector3 position)
    {
        bool xInBounds = position.x < noise.GetLength(0) - 1 && position.x >= 0;
        bool yInBounds = position.y < noise.GetLength(1) - 1 && position.y >= 0;
        bool zInBounds = position.z < noise.GetLength(2) - 1 && position.z >= 0;
        return xInBounds && yInBounds && zInBounds;
    }
    private static void UnloadChunks()
    {
        //Add any chunks beyond the render distance which are loaded to the unloaded chunks and unload them.
        foreach(var chunk in allChunks)
        {
            var xDist = Mathf.Abs(chunk.Key.x / Chunk.WIDTH - playerPosition.x / Chunk.WIDTH);
            var yDist = Mathf.Abs(chunk.Key.y / Chunk.HEIGHT - playerPosition.y / Chunk.HEIGHT);
            var zDist = Mathf.Abs(chunk.Key.z / Chunk.DEPTH - playerPosition.z / Chunk.DEPTH);
            if (xDist > RENDER_DISTANCE || yDist > RENDER_DISTANCE || zDist > RENDER_DISTANCE)
            {
                Chunk tmpChunk;
                if (loadedChunks.TryGetValue(chunk.Key, out tmpChunk))
                {
                    loadedChunks.Remove(chunk.Key);
                    unloadedChunks.Add(chunk.Key, chunk.Value);
                }
                
            }
        }

        foreach(var chunk in unloadedChunks)
        {
            chunk.Value.go.SetActive(false);
        }
    }
    private static void RenderChunks()
    {
        foreach(var kvp in loadedChunks)
        {
            if(kvp.Value.canRender)
                kvp.Value.Render();
        }
    }
    #endregion


}