using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerrain
{
    //render of chunks each direction
    public const int RENDER_DISTANCE = 2; //16
    public const int CENTERED_WIDTH = (2 * RENDER_DISTANCE) + 1;
    public const int CENTERED_HEIGHT = (2 * RENDER_DISTANCE) + 1;
    public const int CENTERED_DEPTH = (2 * RENDER_DISTANCE) + 1;
    
    public const int WIDTH =  CENTERED_WIDTH  + 1;  //40  
    public const int DEPTH =  CENTERED_DEPTH  + 1;  //40
    public const int HEIGHT = CENTERED_HEIGHT + 1;  //16

    //noise
    //public const int NOISE_WIDTH =  ((2 * (2 * RENDER_DISTANCE)) + 1) * Chunk.WIDTH + 1;
    //public const int NOISE_HEIGHT = ((2 * (2 * RENDER_DISTANCE)) + 1) * Chunk.HEIGHT + 1;
    //public const int NOISE_DEPTH =  ((2 * (2 * RENDER_DISTANCE)) + 1) * Chunk.DEPTH + 1;
    public const int NOISE_WIDTH = WIDTH * Chunk.WIDTH + 1;
    public const int NOISE_DEPTH = DEPTH * Chunk.DEPTH + 1;
    public const int NOISE_HEIGHT = HEIGHT * Chunk.HEIGHT + 1;

    //public static double[] noise = new double[NOISE_WIDTH * NOISE_HEIGHT * NOISE_DEPTH];
    public static double[,,] noise = new double[NOISE_WIDTH, NOISE_HEIGHT, NOISE_DEPTH];
    public static double isolevel = 0.5;
    private static double min = double.MaxValue;
    private static double max = double.MinValue;
    private static int normalizeValue = 1;


    private static Dictionary<Vector3, Chunk> activeChunks = new Dictionary<Vector3, Chunk>();
    private static Vector3 playerPosition;

    //debug stuff
    public bool showGizmos;


    public static void Generate()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        GenerateHeightMap();
    }
   
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
    private static void GenerateHeightMap()
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
                    var octaves = 3;
                    var deltaFrequency = 0.5;
                    var deltaAmplitude = 2.0;
                    var deltaScale = 1.0;

                    var frequency = 2.0;
                    var amplitude = 10.0;
                    var scale = 5.0;

                    double sample = 0.0;
                    for (int i = 0; i < octaves; i++)
                    {

                        sample += simplexNoise.eval(((double)x / (double)Chunk.WIDTH) * frequency,
                                                    ((double)y / (double)Chunk.HEIGHT) * frequency,
                                                    ((double)z / (double)Chunk.DEPTH) * frequency) * amplitude * scale - y * 3f;

                        frequency *= deltaFrequency;
                        amplitude *= deltaAmplitude;
                        scale *= deltaScale;
                    }

                    if (sample < min) min = sample;
                    if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
                    noise[x, y, z] = sample;

                    #endregion

                    #region giant cube example
                    //double sample = 1.0;

                    //if (x == 0 || x == NOISE_WIDTH - 1 || y == 0 || y == NOISE_HEIGHT - 1 || z == 0 || z == NOISE_DEPTH - 1)
                    //    sample = -1.0;

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    ////noise[index] = sample;
                    //noise[x, y, z] = sample;
                    #endregion

                    #region flat
                    //double sample = -1.0;

                    ////if (y <= (HEIGHT / 2) * Chunk.HEIGHT)
                    //if (y < 17)
                    //{
                    //    sample = 1.0;
                    //}

                    //if (sample < min) min = sample;
                    //if (sample > max) max = sample;

                    //var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    //noise[index] = sample;
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
                var chunkX = (x + playerX) * Chunk.WIDTH;
                var chunkY = y * Chunk.HEIGHT;
                if (playerY < 0) chunkY -= Chunk.HEIGHT;
                var chunkZ = (z + playerZ) * Chunk.DEPTH;
                var newChunkPosition = new Vector3(chunkX, chunkY, chunkZ);

                var noiseX = (x + playerX + (NOISE_WIDTH - 1) / Chunk.WIDTH / 2) * Chunk.WIDTH;
                var noiseY = (y + (NOISE_HEIGHT - 1) / Chunk.HEIGHT / 2) * Chunk.HEIGHT;
                if (playerY < 0) noiseY -= Chunk.HEIGHT;
                var noiseZ = (z + playerZ + (NOISE_DEPTH - 1) / Chunk.DEPTH / 2) * Chunk.DEPTH;
                var newChunkNoisePosition = new Vector3(noiseX, noiseY, noiseZ);

                if (!ChunkExists(newChunkPosition) && NoiseExists(newChunkNoisePosition))
                {
                    var go = new GameObject(newChunkPosition.ToString());
                    go.transform.position = newChunkPosition;

                    var chunk = new Chunk(go, newChunkNoisePosition);
                    activeChunks.Add(go.transform.position, chunk);
                    chunk.GenerateTerrain();
                    Debug.Log("generated");
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
            if (y == (Y + playerY) / 2)
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

    private static bool ChunkExists(Vector3 position)
    {
        Chunk tmpChunk;
        return activeChunks.TryGetValue(position, out tmpChunk);
    }
    private static bool NoiseExists(Vector3 position)
    {
        bool xInBounds = position.x < noise.GetLength(0) - 1 && position.x >= 0;
        bool yInBounds = position.y < noise.GetLength(1) - 1 && position.y >= 0;
        bool zInBounds = position.z < noise.GetLength(2) - 1 && position.z >= 0;
        return xInBounds && yInBounds && zInBounds;
    }
    private static void RenderChunks()
    {
        foreach(var kvp in activeChunks)
        {
            kvp.Value.Render();
        }
    }

    public static void Update()
    {
        //if (InputManager.isSpace)
        {
            GenerateChunks();
            RenderChunks();
        }
    }
    #endregion


}