using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WanderingEpisode : Episode
{
    private int count;
    private int emergencyExitCount;
    private bool emergencyExit = false;
    private Vector2 previousUserPosition;
    private Vector2 currentTileLocationVector = new Vector2(0f,0f);
    private Vector2 nextTileLocationVector = new Vector2(0f,0f);
    private Vector2 restoreVector = new Vector2(0f,0f);
    private Vector2 resetPoint = new Vector2(0f,0f);
    private string resetType = "";

    private int currentTileNumber;
    private int Horizontal;
    private int Vertical;
    private int currentHorizontal;
    private int currentVertical;
    public bool skipBit = false;
    public bool resetMode = false;
    public bool pathRestoreMode = false;
    private float bound;

    private Vector2 a;
    private Vector2 b;
    private Vector2 c;
    private Vector2 d;

    private Vector2 rightPoint1;
    private Vector2 topPoint1;
    private Vector2 leftPoint1;
    private Vector2 bottomPoint1;

    private Vector2 rightPoint2;
    private Vector2 topPoint2;
    private Vector2 leftPoint2;
    private Vector2 bottomPoint2;

    public WanderingEpisode() : base() { }

    public WanderingEpisode(int episodeLength) : base(episodeLength) { }


    protected override void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace)
    {
        Vector2 samplingPosition = Vector2.zero;
        Vector2 sampleForward = Vector2.zero;
        Vector2 userPosition = virtualUserTransform.localPosition;
        
        count = 0;
        bound = 0.4f;

        if (GetCurrentEpisodeIndex() <= 1)
        {
            previousUserPosition = virtualAgentInitialPosition;
        }

        if(virtualSpace.tileMode)
        {
            skipBit = false;
            currentTileNumber = Convert.ToInt32(virtualSpace.spaceObject.gameObject.name.Replace("tile_",""));
            Polygon2D currentTile = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber];
            currentTileLocationVector = currentTile.transform2D.localPosition;

            Horizontal = virtualSpace.tileAreaSetting[0];
            Vertical = virtualSpace.tileAreaSetting[1];

            currentVertical = (int) (currentTileNumber/(4*Horizontal));
            currentHorizontal = currentTileNumber % (4*Horizontal);

            float resetLength = 0.2f;


            do
            {
                count++;
            
                // float angle = Utility.sampleNormal(0f, 18f, -180f, 180f);
                // float distance = 0.2f;

                float angle = Utility.sampleUniform(-135.0f, 135.0f);
                float distance = Utility.sampleUniform(0.2f, 4.0f);

                sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
                samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
                

                // Debug.Log("Sampling Position: " + samplingPosition);

                // Polygon2D currentTile1 = (Polygon2D) virtualSpace.spaceObjects[0];
                // Debug.Log("0 Tile localPosition: " + currentTile1.transform2D.localPosition);

                // Polygon2D currentTile2 = (Polygon2D) virtualSpace.spaceObjects[1];
                // Debug.Log("1 Tile localPosition: " + currentTile2.transform2D.localPosition);

                // Polygon2D currentTile3 = (Polygon2D) virtualSpace.spaceObjects[2];
                // Debug.Log("2 Tile localPosition: " + currentTile3.transform2D.localPosition);

                // Polygon2D currentTile4 = (Polygon2D) virtualSpace.spaceObjects[3];
                // Debug.Log("3 Tile localPosition: " + currentTile4.transform2D.localPosition);


                a = virtualSpace.tileCrossingVectors[0];
                b = virtualSpace.tileCrossingVectors[1];
                c = virtualSpace.tileCrossingVectors[2];
                d = virtualSpace.tileCrossingVectors[3];
                // Debug.Log("a: " + a);
                // Debug.Log("b: " + b);
                // Debug.Log("c: " + c);
                // Debug.Log("d: " + d);

                rightPoint1 = (samplingPosition - a - currentTileLocationVector);
                topPoint1 = (samplingPosition - b - currentTileLocationVector);
                leftPoint1 = (samplingPosition - c - currentTileLocationVector);
                bottomPoint1 = (samplingPosition - d - currentTileLocationVector);

                rightPoint2 = (samplingPosition + c - currentTileLocationVector);
                topPoint2 = (samplingPosition + d - currentTileLocationVector);
                leftPoint2 = (samplingPosition + a - currentTileLocationVector);
                bottomPoint2 = (samplingPosition + b - currentTileLocationVector);


                if(resetMode)
                {
                    if(resetType == "1R")
                    {
                        samplingPosition = resetPoint + new Vector2(resetLength, 0);
                        currentTargetPosition = resetPoint + new Vector2(resetLength, 0);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 1];
                        // Debug.Log("currentTargetPosition2: "+currentTargetPosition);
                        // Debug.Log("User local Position2: "+virtualUserTransform.localPosition);
                    }
                    else if(resetType == "2R")
                    {
                        samplingPosition = resetPoint + new Vector2(resetLength, 0);
                        currentTargetPosition = resetPoint + new Vector2(resetLength, 0);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 3];
                    }
                    else if(resetType == "1T" || resetType == "2T")
                    {
                        samplingPosition = resetPoint + new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2];
                    }
                    else if(resetType == "1L")
                    {
                        samplingPosition = resetPoint - new Vector2(resetLength, 0);
                        currentTargetPosition = resetPoint - new Vector2(resetLength, 0);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 3];
                    }
                    else if(resetType == "2L")
                    {
                        samplingPosition = resetPoint - new Vector2(resetLength, 0);
                        currentTargetPosition = resetPoint - new Vector2(resetLength, 0);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 1];
                    }
                    else if(resetType == "1B" || resetType == "2B")
                    {
                        samplingPosition = resetPoint - new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 2];
                    }

                    // else if(resetType == "2T")
                    // {
                    //     samplingPosition = resetPoint + new Vector2(0, resetLength);
                    //     currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                    //     virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2];
                    // }

                    // else if(resetType == "2B")
                    // {
                    //     samplingPosition = resetPoint - new Vector2(0, resetLength);
                    //     currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                    //     virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 2];
                    // }
                    else if(resetType == "3T" || resetType == "4T")
                    {
                        samplingPosition = resetPoint + new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 2];
                    }
                    else if(resetType == "3B" || resetType == "4B")
                    {
                        samplingPosition = resetPoint - new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                        virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2];
                    }
                    // else if(resetType == "4T")
                    // {
                    //     samplingPosition = resetPoint + new Vector2(0, resetLength);
                    //     currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                    //     virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 2];
                    // }
                    // else if(resetType == "4B")
                    // {
                    //     samplingPosition = resetPoint - new Vector2(0, resetLength);
                    //     currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                    //     virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2];
                    // }

                    resetMode = false;
                    pathRestoreMode = true;
                    break;
                }
                if(pathRestoreMode)
                {
                    samplingPosition = restoreVector;
                    currentTargetPosition = restoreVector;
                    pathRestoreMode = false;
                    break;
                }

                if(currentTileNumber % 4 == 0 ) // Type 1
                {
                    Polygon2D rightTile1 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber+1];
                    Polygon2D topTile1 = null;
                    Polygon2D bottomTile1 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber+2];
                    Polygon2D leftTile1 = null;
                    
                    bool firstRow = false;
                    bool firstColumn = false;
                    if( currentTileNumber - 4*Horizontal + 2 < 0)
                    {
                        firstRow = true;
                    }
                    else
                    {
                        topTile1 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber-4*Horizontal+2];
                    }

                    if( currentTileNumber - 3 < 0)
                    {
                        firstColumn = true;
                    }
                    else
                    {
                        leftTile1 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber-3];
                    }
                    // Debug.Log("R: "+virtualSpace.spaceObjects[currentTileNumber + 1].IsInsideTile(samplingPosition, rightTile1.transform2D.localPosition, Space.Self, this.bound));
                    // Debug.Log("U: "+virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f));
                    // Debug.Log("L: "+virtualSpace.spaceObjects[currentTileNumber - 3].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f));
                    // Debug.Log("B: "+virtualSpace.spaceObjects[currentTileNumber + 2].IsInsideTile(samplingPosition, bottomTile1.transform2D.localPosition, Space.Self, this.bound));
                    
                    if(virtualSpace.spaceObjects[currentTileNumber + 1].IsInsideTile(samplingPosition, rightTile1.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber + 1].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 ) //오른쪽 전환인 경우 V    
                    //if( rightPoint1.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) //오른쪽 전환인 경우 V
                    {
                        Polygon2D nextTile = rightTile1;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber+1];
                        
                        nextTileLocationVector = nextTile.transform2D.localPosition;

                        //Debug.Log("currentTileLocationVector: " + currentTileLocationVector);
                        //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                        // //이동을 시킴
                        // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                        // Vector2 moveLength = 2*(currentTileLocationVector + a - virtualUserTransform.localPosition);

                        // samplingPosition = userPosition + moveLength;
                        // currentTargetPosition = userPosition + moveLength;
                        // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 1];
                        // skipBit = true;

                        restoreVector = samplingPosition;
                        resetPoint = currentTileLocationVector + a;
                        Debug.Log("resetPoint: " + resetPoint);
                        samplingPosition = resetPoint - new Vector2( resetLength, 0);
                        currentTargetPosition = resetPoint - new Vector2( resetLength, 0);
                        Debug.Log("currentTargetPosition1: " + currentTargetPosition);
                        Debug.Log("User local Position1: "+virtualUserTransform.localPosition);
                        //skipBit = true;
                        resetMode = true;
                        resetType = "1R";
                        break;
                    }
                    if(!firstRow) // 위쪽 전환인 경우 X
                    {
                        if(virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2].IsInsideTile(samplingPosition, topTile1.transform2D.localPosition, Space.Self, this.bound)
                        && virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 ) // 위쪽 전환인 경우 X
                        //if(topPoint1.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 위쪽 전환인 경우 X
                        {
                            
                            Polygon2D nextTile = topTile1;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber-4*Horizontal+2];
                            
                            nextTileLocationVector = nextTile.transform2D.localPosition;
                            //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                            // // 이동을 시킴
                            // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                            // Vector2 moveLength = 2*(currentTileLocationVector + b - virtualUserTransform.localPosition);

                            // samplingPosition = userPosition + moveLength;
                            // currentTargetPosition = userPosition + moveLength;
                            // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2];
                            // skipBit = true;

                            restoreVector = samplingPosition;
                            resetPoint = currentTileLocationVector + b;
                            samplingPosition = resetPoint - new Vector2(0, resetLength);
                            currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                            resetMode = true;
                            resetType = "1T";
                            break;
                        }
                    }
                    if(!firstColumn) // 왼쪽인 경우 X
                    {
                        if(virtualSpace.spaceObjects[currentTileNumber - 3].IsInsideTile(samplingPosition, leftTile1.transform2D.localPosition, Space.Self, this.bound)
                        && virtualSpace.spaceObjects[currentTileNumber - 3].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 ) // 왼쪽인 경우 X
                        //if(leftPoint1.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 왼쪽인 경우 X
                        {
                            
                            Polygon2D nextTile = leftTile1;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber-3];
                            
                            nextTileLocationVector = nextTile.transform2D.localPosition;
                            //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);
                            
                            // // 이동을 시킴
                            // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                            // Vector2 moveLength = 2*(currentTileLocationVector + c - virtualUserTransform.localPosition);

                            // samplingPosition = userPosition + moveLength;
                            // currentTargetPosition = userPosition + moveLength;
                            // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 3];
                            // skipBit = true;
                            // break;
                            restoreVector = samplingPosition;
                            resetPoint = currentTileLocationVector + c;
                            samplingPosition = resetPoint + new Vector2(resetLength, 0);
                            currentTargetPosition = resetPoint + new Vector2(resetLength, 0);
                            resetMode = true;
                            resetType = "1L";
                            break;
                        }
                    }
                    if(virtualSpace.spaceObjects[currentTileNumber + 2].IsInsideTile(samplingPosition, bottomTile1.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber + 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 ) // 아랫쪽인 경우 V
                    //if( bottomPoint1.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 아랫쪽인 경우 V
                    {
                        
                        Polygon2D nextTile = bottomTile1;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber+2];
                        
                        nextTileLocationVector = nextTile.transform2D.localPosition;
                        //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);
                        
                        // // 이동을 시킴
                        // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                        // Vector2 moveLength = 2*(currentTileLocationVector + d - virtualUserTransform.localPosition);

                        // samplingPosition = userPosition + moveLength;
                        // currentTargetPosition = userPosition + moveLength;
                        // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 2];
                        // skipBit = true;
                        // break;
                        restoreVector = samplingPosition;
                        resetPoint = currentTileLocationVector + d;
                        samplingPosition = resetPoint + new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                        resetMode = true;
                        resetType = "1B";
                        break;
                    }

                    Debug.Log("Type 1 Done");

                }
                else if(currentTileNumber % 4 == 1) // Type 2
                {
                    Polygon2D rightTile2 = null;
                    Polygon2D topTile2 = null;
                    Polygon2D leftTile2 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber - 1];
                    Polygon2D bottomTile2 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber + 2];

                    bool firstRow = false;
                    bool lastColumn = false;
                    if( currentTileNumber - 4*Horizontal + 2 < 0)
                    {
                        firstRow = true;
                    }
                    else
                    {
                        topTile2 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2];
                    }
                    if( (currentTileNumber+3)/4 % Horizontal == 0 )
                    {
                        lastColumn = true;
                    }
                    else
                    {
                        rightTile2 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber+3];
                    }

                    if(!lastColumn) //오른쪽 전환인 경우 X
                    {
                        if(virtualSpace.spaceObjects[currentTileNumber + 3].IsInsideTile(samplingPosition, rightTile2.transform2D.localPosition, Space.Self, this.bound)
                        && virtualSpace.spaceObjects[currentTileNumber + 3].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                        //if(rightPoint2.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound))
                        {
                            Polygon2D nextTile = rightTile2;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber+3];
                            
                            nextTileLocationVector = nextTile.transform2D.localPosition;
                            //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                            // // 이동을 시킴
                            // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                            // Vector2 moveLength = 2*(currentTileLocationVector -c - virtualUserTransform.localPosition);

                            // samplingPosition = userPosition + moveLength;
                            // currentTargetPosition = userPosition + moveLength;
                            // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 3];
                            // skipBit = true;
                            // break;

                            restoreVector = samplingPosition;
                            resetPoint = currentTileLocationVector - c;
                            samplingPosition = resetPoint - new Vector2(resetLength, 0);
                            currentTargetPosition = resetPoint - new Vector2(resetLength, 0);
                            resetMode = true;
                            resetType = "2R";
                            break;
                        }

                    }
                    if(!firstRow) // 위쪽 전환인 경우 X
                    {
                        if(virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2].IsInsideTile(samplingPosition, topTile2.transform2D.localPosition, Space.Self, this.bound)
                        && virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                        //if(topPoint2.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound)) // 위쪽 전환인 경우 X
                        {
                            Polygon2D nextTile = topTile2;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2];
                            
                            nextTileLocationVector = nextTile.transform2D.localPosition;
                            //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);
                            
                            // // 이동을 시킴
                            // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                            // Vector2 moveLength = 2*(currentTileLocationVector -d - virtualUserTransform.localPosition);

                            // samplingPosition = userPosition + moveLength;
                            // currentTargetPosition = userPosition + moveLength;
                            // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 4*Horizontal + 2];
                            // skipBit = true;
                            // break;

                            restoreVector = samplingPosition;
                            resetPoint = currentTileLocationVector - d;
                            samplingPosition = resetPoint - new Vector2(0, resetLength);
                            currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                            resetMode = true;
                            resetType = "2T";
                            break;
                        }
                    }
                    if(virtualSpace.spaceObjects[currentTileNumber - 1].IsInsideTile(samplingPosition, leftTile2.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber - 1].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                    //if(leftPoint2.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 왼쪽인 경우 V
                    {
                        //Debug.Log("Left distance: " + leftPoint2.magnitude);
                        
                        Polygon2D nextTile = leftTile2;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber - 1];
                        
                        nextTileLocationVector = nextTile.transform2D.localPosition;
                        //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                        // // 이동을 시킴
                        // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                        // Vector2 moveLength = 2*(currentTileLocationVector -a - virtualUserTransform.localPosition);

                        // samplingPosition = userPosition + moveLength;
                        // currentTargetPosition = userPosition + moveLength;
                        // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 1];
                        // skipBit = true;
                        // break;

                        restoreVector = samplingPosition;
                        resetPoint = currentTileLocationVector - a;
                        samplingPosition = resetPoint + new Vector2(resetLength, 0);
                        currentTargetPosition = resetPoint + new Vector2(resetLength, 0);
                        resetMode = true;
                        resetType = "2L";
                        break;
                    }
                    if(virtualSpace.spaceObjects[currentTileNumber + 2].IsInsideTile(samplingPosition, bottomTile2.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber + 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                    //if(bottomPoint2.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 아랫쪽인 경우 V
                    {
                        //Debug.Log("Bottom distance: " + bottomPoint2.magnitude);

                        Polygon2D nextTile = bottomTile2;// (Polygon2D) virtualSpace.spaceObjects[currentTileNumber + 2];
                        
                        nextTileLocationVector = nextTile.transform2D.localPosition;
                        //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                        // // 이동을 시킴
                        // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                        // Vector2 moveLength = 2*(currentTileLocationVector -b - virtualUserTransform.localPosition);

                        // samplingPosition = userPosition + moveLength;
                        // currentTargetPosition = userPosition + moveLength;
                        // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 2];
                        // skipBit = true;
                        // break;

                        restoreVector = samplingPosition;
                        resetPoint = currentTileLocationVector - b;
                        samplingPosition = resetPoint + new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                        resetMode = true;
                        resetType = "2B";
                        break;
                    }
                    Debug.Log("Type 2 Done");

                }
                else if(currentTileNumber % 4 == 2) // Type 3
                {
                    Polygon2D topTile3 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber - 2];
                    Polygon2D bottomTile3 = null;

                    bool lastRow = false;
                    if( currentTileNumber + 4*Horizontal - 2 > 4*(Vertical*Horizontal-1) )
                    {
                        lastRow = true;
                    }
                    else
                    {
                        bottomTile3 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2];
                    }

                    if(virtualSpace.spaceObjects[currentTileNumber - 2].IsInsideTile(samplingPosition, topTile3.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber - 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                    //if(topPoint2.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 위쪽 전환인 경우 V
                    {
                        
                        Polygon2D nextTile = topTile3;// (Polygon2D) virtualSpace.spaceObjects[currentTileNumber - 2];
                        
                        nextTileLocationVector = nextTile.transform2D.localPosition;
                        //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                        // // 이동을 시킴
                        // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                        // Vector2 moveLength = 2*(currentTileLocationVector -d - virtualUserTransform.localPosition);

                        // samplingPosition = userPosition + moveLength;
                        // currentTargetPosition = userPosition + moveLength;
                        // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 2];
                        // skipBit = true;
                        // break;

                        restoreVector = samplingPosition;
                        resetPoint = currentTileLocationVector - d;
                        samplingPosition = resetPoint - new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                        resetMode = true;
                        resetType = "3T";
                        break;
                    }
                    if(virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2].IsInsideTile(samplingPosition, bottomTile3.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                    //if(bottomPoint2.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 아랫쪽인 경우 X
                    {
                        if(!lastRow) // 아랫쪽인 경우 X
                        {
                            Polygon2D nextTile = bottomTile3;// (Polygon2D) virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2];
                            
                            nextTileLocationVector = nextTile.transform2D.localPosition;
                            //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);

                            // // 이동을 시킴
                            // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                            // Vector2 moveLength = 2*(currentTileLocationVector -b - virtualUserTransform.localPosition);

                            // samplingPosition = userPosition + moveLength;
                            // currentTargetPosition = userPosition + moveLength;
                            // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2];
                            // skipBit = true;
                            // break;

                            restoreVector = samplingPosition;
                            resetPoint = currentTileLocationVector - b;
                            samplingPosition = resetPoint + new Vector2(0, resetLength);
                            currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                            resetMode = true;
                            resetType = "3B";
                            break;
                        }
                    }
                    Debug.Log("Type 3 Done");
                }
                else if(currentTileNumber % 4 == 3) // Type 4
                {
                    Polygon2D topTile4 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber-2];
                    Polygon2D bottomTile4 = null;

                    bool lastRow = false;
                    if( currentTileNumber + 4*Horizontal - 2 > 4*(Vertical*Horizontal-1) )
                    {
                        lastRow = true;
                    }
                    else
                    {
                        bottomTile4 = (Polygon2D) virtualSpace.spaceObjects[currentTileNumber+4*Horizontal-2];
                    }

                    if(virtualSpace.spaceObjects[currentTileNumber - 2].IsInsideTile(samplingPosition, topTile4.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber - 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 ) // 위쪽 전환인 경우 V
                    //if(topPoint1.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 위쪽 전환인 경우 V
                    {
                        Polygon2D nextTile = topTile4;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber-2];
                        
                        nextTileLocationVector = nextTile.transform2D.localPosition;
                        //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);
                        
                        // // 이동을 시킴
                        // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                        // Vector2 moveLength = 2*(currentTileLocationVector + b - virtualUserTransform.localPosition);

                        // samplingPosition = userPosition + moveLength;
                        // currentTargetPosition = userPosition + moveLength;
                        // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber - 2];
                        // skipBit = true;
                        // break;

                        restoreVector = samplingPosition;
                        resetPoint = currentTileLocationVector + b;
                        samplingPosition = resetPoint - new Vector2(0, resetLength);
                        currentTargetPosition = resetPoint - new Vector2(0, resetLength);
                        resetMode = true;
                        resetType = "4T";
                        break;
                    }
                    if(virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2].IsInsideTile(samplingPosition, bottomTile4.transform2D.localPosition, Space.Self, this.bound)
                    && virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2].NumOfIntersect(userPosition, samplingPosition, Space.Self, "default", 0f) == 1 )
                    //if(bottomPoint1.magnitude < resetLength && virtualSpace.IsInsideTile(samplingPosition, currentTileLocationVector, Space.Self, this.bound) ) // 아랫쪽인 경우 X
                    {
                        if(!lastRow) // 아랫쪽인 경우 X
                        {
                            Polygon2D nextTile = bottomTile4;//(Polygon2D) virtualSpace.spaceObjects[currentTileNumber+4*Horizontal-2];
                            
                            nextTileLocationVector = nextTile.transform2D.localPosition;
                            //Debug.Log("Tile Assigning Vector: " + nextTileLocationVector);
                            
                            // // 이동을 시킴
                            // Vector2 moveStartPosition = virtualUserTransform.localPosition;
                            // Vector2 moveLength = 2*(currentTileLocationVector + d - virtualUserTransform.localPosition);

                            // samplingPosition = userPosition + moveLength;
                            // currentTargetPosition = userPosition + moveLength;
                            // virtualSpace.spaceObject = virtualSpace.spaceObjects[currentTileNumber + 4*Horizontal - 2];
                            // skipBit = true;
                            // break;

                            restoreVector = samplingPosition;
                            resetPoint = currentTileLocationVector + d;
                            samplingPosition = resetPoint + new Vector2(0, resetLength);
                            currentTargetPosition = resetPoint + new Vector2(0, resetLength);
                            resetMode = true;
                            resetType = "4B";
                            break;
                        }
                    }
                    Debug.Log("Type 4 Done");
                }
                
                if (count >= 50)
                {
                    angle = Utility.sampleUniform(90f, 270f);
                    count = 1;
                    emergencyExitCount++;

                    if(emergencyExitCount == 5)
                    {
                        emergencyExit = true;
                        emergencyExitCount = 0;

                        Vector2 movedVector1 = previousUserPosition - userPosition;
                        Vector2 movedVector2 = userPosition - virtualAgentInitialPosition;
                        
                        if (movedVector1.magnitude < distance-0.001f && movedVector2.magnitude < distance - 0.001f) // 이전과 현재 위치가 같으면서 초기 위치일 때
                        {
                            SetWrongEpisode(true);
                            SetCurrentEpisodeIndex(GetEpisodeLength());
                            // Debug.Log("Wrong Initialized Episode!");
                            // sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
                            // samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
                        }

                        break;
                    }
                }
                // Debug.Log("Sampling Position: " + samplingPosition);
            //} while ( (!virtualSpace.IsInsideTile(samplingPosition, nextTileLocationVector, Space.Self, this.bound) || !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self) ) && !skipBit); // !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self, 0.2f)
            //} while ( (!virtualSpace.IsInsideTile(samplingPosition, nextTileLocationVector, Space.Self, this.bound)) && !skipBit); // !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self, 0.2f)
            } while ( !virtualSpace.IsInsideTile(samplingPosition, nextTileLocationVector, Space.Self, this.bound));

        }
        else if(!virtualSpace.tileMode)
        {
            do
            {
                count++;
            
                float angle = Utility.sampleNormal(0f, 18f, -180f, 180f);
                float distance = 0.2f;
                //float distance = Utility.sampleNormal(0.4f, 2f, 0.25f, 5f); Distance도 랜덤. 깊숙히 안들어가는 문제 약간 보임.
                // Debug.Log(-virtualUserTransform.localPosition * Time.fixedDeltaTime);

                if (count >= 100)
                {
                    angle = Utility.sampleUniform(90f, 270f);
                    count = 1;
                    emergencyExitCount++;
                    
                    if(emergencyExitCount == 5)
                    {
                        emergencyExit = true;
                        emergencyExitCount = 0;

                        Vector2 movedVector1 = previousUserPosition - userPosition;
                        Vector2 movedVector2 = userPosition - virtualAgentInitialPosition;
                        
                        if (movedVector1.magnitude < distance-0.001f && movedVector2.magnitude < distance - 0.001f) // 이전과 현재 위치가 같으면서 초기 위치일 때
                        {
                            SetWrongEpisode(true);
                            SetCurrentEpisodeIndex(GetEpisodeLength());
                            // Debug.Log("Wrong Initialized Episode!");
                            // sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
                            // samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
                        }

                        break;
                    }
                }
                sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
                samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준

            } while (!virtualSpace.IsInside(samplingPosition, Space.Self, 0.5f)); // !virtualSpace.IsPossiblePath(samplingPosition, userPosition, Space.Self, 0.2f)
        }

        if (emergencyExit)
        {
            emergencyExit = false;

            // float angle = Utility.sampleNormal(0f, 18f, -180f, 180f);
            // float distance = 0.2f;
            // Vector2 sampleForward = Utility.RotateVector2(virtualUserTransform.forward, angle);
            // samplingPosition = userPosition + sampleForward * distance; // local 좌표계에서 절대 위치 기준
            if(GetWrongEpisode())
            {
                currentTargetPosition = userPosition;
            }
            else
            {
                currentTargetPosition = previousUserPosition;
            }
            
        }
        else
        {
            count = 1;
            previousUserPosition = userPosition;
            currentTargetPosition = samplingPosition;
        }

        // Vector2 initialToTarget = previousUserPosition - virtualUserTransform.localPosition;
        // float InitialAngle = Vector2.SignedAngle(virtualUserTransform.forward, initialToTarget);
        // float initialDistance = Vector2.Distance(virtualUserTransform.localPosition, previousUserPosition);
        // float initialAngleDirection = Mathf.Sign(InitialAngle);

        // Vector2 virtualTargetDirection = Matrix3x3.CreateRotation(InitialAngle) * virtualUserTransform.forward; // target을 향하는 direction(forward)를 구함
        // Vector2 virtualTargetPosition = virtualUserTransform.localPosition + virtualTargetDirection * initialDistance; // target에 도달하는 position을 구함

        // float maxRotTime = Mathf.Abs(InitialAngle) / 500;
        // float maxTransTime = initialDistance / 4;
        // float remainRotTime = 0;
        // float remainTransTime = 0;

        // if (maxTransTime - remainTransTime > 0.06f)
        // {
        //     Debug.Log(maxTransTime - remainTransTime);
        // }
    }
}
