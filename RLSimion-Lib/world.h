#pragma once

class CNamedVarSet;
typedef CNamedVarSet CState;
typedef CNamedVarSet CAction;
class CParameters;
class CDynamicModel;

class CWorld
{
	CDynamicModel* m_pDynamicModel;
	int m_simulationSteps;
	static double m_dt;
	static double m_t;
	static double m_step_start_t;
public:
	static double getDT();
	static double getT();
	static double getStepStartT();

	CWorld(char* configFile);
	~CWorld();

	CState *getStateInstance(); // pointer to a new CState. has to be deleted
	CAction *getActionInstance();

	CState *getStateDescriptor(); // pointer to the state descriptor. don't delete
	CAction *getActionDescriptor();

	void reset(CState *s);
	void executeAction(CState *s,CAction *a,CState *s_p);
};

class CDynamicModel
{
protected:
	CState *m_pStateDescriptor;
	CAction *m_pActionDescriptor;
public:
	virtual void reset(CState *s)= 0;
	virtual void executeAction(CState *s,CAction *a,double dt)= 0;

	CState* getStateDescriptor(){return m_pStateDescriptor;}
	CAction* getActionDescriptor(){return m_pActionDescriptor;}
};
