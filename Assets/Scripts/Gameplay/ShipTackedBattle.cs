using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTackedBattle
{
    public int Result;
    public int Ship1Damaged;
    public int Ship2Damaged;
    public Ship ship1;
    public Ship ship2;

    public int WinDamaged;
    public int LoseDamaged;

    public float surrenderRate = 0.2f;
    public float atkHealthRate = 0.1f;
    public ShipTackedBattle(Ship ship1, Ship ship2)
    {
        this.ship1 = ship1;
        this.ship2 = ship2;
    }

    public void CalculateResult()
    {
        if (ship1.curShipData.hullHealth == 0 || ship2.curShipData.hullHealth == 0)
        {
            //hull health = 0 crew surrender
            if (ship1.curShipData.hullHealth == 0)
            {
                ship1.EnterStateDefeated();
                ship1.Group = ship2.Group;
                Result = 2;
            }
            else
            {
                ship2.EnterStateDefeated();
                ship2.Group = ship1.Group;
                Result = 1;
            }
            return;
        }
        int crew1 = ship1.curShipData.maxCrew;
        int crew2 = ship2.curShipData.maxCrew;
        if (crew1 == crew2)
        {
            //draw result 
            Ship1Damaged = (int)(0.3 * crew1);
            Ship2Damaged = (int)(0.3 * crew2);
            ship1.curShipData.maxCrew -= Ship1Damaged;
            ship2.curShipData.maxCrew -= Ship2Damaged;
            Result = 0;
        }
        else if (crew1 > crew2)
        {
            FakeBattle(crew1, crew2);
            Ship1Damaged = WinDamaged;
            Ship2Damaged = LoseDamaged;
            ship1.curShipData.maxCrew -= Ship1Damaged;
            ship2.curShipData.maxCrew -= Ship2Damaged;
            ship2.EnterStateDefeated();
            ship2.Group = ship1.Group;
            Result = 1;
        }
        else
        {
            FakeBattle(crew2, crew1);
            Ship1Damaged = LoseDamaged;
            Ship2Damaged = WinDamaged;
            ship1.curShipData.maxCrew -= Ship1Damaged;
            ship2.curShipData.maxCrew -= Ship2Damaged;
            ship1.EnterStateDefeated();
            ship1.Group = ship2.Group;
            Result = 2;
        }
    }

    private void FakeBattle(float winCrew, float loseCrew)
    {
        float atkWin = winCrew * atkHealthRate;
        float atkLose = loseCrew * atkHealthRate;
        WinDamaged = (int)winCrew;
        LoseDamaged = (int)loseCrew;
        Debug.Log("Fake battle " + loseCrew + "/" + winCrew);
        while (loseCrew > 0 && (loseCrew / winCrew > surrenderRate))
        {
            Debug.Log("Fake battle " + loseCrew + "/" + winCrew);
            loseCrew -= atkWin;
            winCrew -= atkLose;
            atkWin = winCrew * atkHealthRate;
            atkLose = loseCrew * atkHealthRate;
        }
        Debug.Log("Fake battle " + loseCrew + "/" + winCrew);
        if (loseCrew <= 0) loseCrew = 0;
        WinDamaged = (int)(WinDamaged - winCrew);
        LoseDamaged = (int)(LoseDamaged - loseCrew);
    }
}


