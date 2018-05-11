using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFactory
{

    private Vector3[] position =
    {
        new Vector3(-20,0f,-20),new Vector3(-20,0f,0),new Vector3(-20,0f,20),
        new Vector3(0,0f,-20),new Vector3(0,0f,0),new Vector3(0,0f,20)
    };
    private int no;
    private List<PatrolData> patrols;
    private int loc = 0;

    private static PatrolFactory instance;
    public static PatrolFactory getInstance()
    {
        if (instance == null)
        {
            instance = new PatrolFactory();
        }
        return instance;
    }

    private PatrolFactory()
    {
        no = 0;
        patrols = new List<PatrolData>();
    }

    public void makePatrol(int num)
    {
        for(no = 0;no < num;no++)
        {
            PatrolData temp = new PatrolData(no);
            temp.location = position[no];
            temp.show();
            patrols.Add(temp);
        }
    }

    public PatrolData getPatrol(string no)
    {
        foreach(PatrolData patrol in patrols)
        {
            if (no.Equals(patrol.no.ToString()))
                return patrol;
        }
        return null;
    }

    public void follow(int no)
    {
        if (loc != 0)
            patrols[loc - 1].follow = false;
        patrols[no - 1].follow = true;
        loc = no;
    }

    public void reSet()
    {
        patrols[loc - 1].follow = false;
        loc = 0;
        no = 0;
        foreach (PatrolData patrol in patrols)
        {
            patrol.patrol.transform.position = position[no];
            no++;
        }
    }
}
