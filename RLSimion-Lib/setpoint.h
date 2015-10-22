#pragma once

class CSetPoint
{
public:
	CSetPoint(){}
	virtual ~CSetPoint(){}

	virtual double getPointSet(double time)= 0;
};

class CFileSetPoint: public CSetPoint
{
	int m_numSteps;
	double *m_pSetPoints;
	double *m_pTimes;
	double m_totalTime;
public:
	CFileSetPoint(char* pFilename);
	~CFileSetPoint();

	double getPointSet(double time);
};

class CFixedStepSizeSetPoint: public CSetPoint
{
	double m_stepTime;
	double m_lastStepTime;
	double m_lastSetPoint;
	double m_min, m_max;

public:
	CFixedStepSizeSetPoint(double stepTime, double min, double max);
	~CFixedStepSizeSetPoint();

	double getPointSet(double time);
};