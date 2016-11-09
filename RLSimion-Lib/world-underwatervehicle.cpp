#include "stdafx.h"
#include "world-underwatervehicle.h"
#include "named-var-set.h"
#include "setpoint.h"
#include "config.h"
#include "world.h"
#include "globals.h"
#include "app.h"
#include "reward.h"


CLASS_CONSTRUCTOR(CUnderwaterVehicle,const char* worldDefinition)
: CDynamicModel(worldDefinition)
{
	CState *pStateDescriptor = getStateDescriptor();
	m_sVSetpoint = pStateDescriptor->getVarIndex("v-setpoint");
	m_sV = pStateDescriptor->getVarIndex("v");
	m_sVDeviation = pStateDescriptor->getVarIndex("v-deviation");

	CAction *pActionDescriptor = getActionDescriptor();
	m_aUThrust = pActionDescriptor->getVarIndex("u-thrust");

	const char* setpointFile;
	FILE_PATH_VALUE(setpointFile, "Set-Point-File", "../config/world/underwater-vehicle/setpoint.txt","The setpoint file");

	if (setpointFile)
		m_pSetpoint= new CFileSetPoint(setpointFile);
	else m_pSetpoint = 0;

	m_pRewardFunction->addRewardComponent(new CToleranceRegionReward("v-deviation", 0.1, 1.0));
	m_pRewardFunction->initialize();
	
	END_CLASS();
}

CUnderwaterVehicle::~CUnderwaterVehicle()
{
	delete m_pSetpoint;
}



void CUnderwaterVehicle::reset(CState *s)
{
	s->setValue(m_sVSetpoint,m_pSetpoint->getPointSet(0.0));
	s->setValue(m_sVDeviation,m_pSetpoint->getPointSet(0.0));
	s->setValue(m_sV,0.0);
}

void CUnderwaterVehicle::executeAction(CState *s,const CAction *a,double dt)
{
	double newSetpoint = m_pSetpoint->getPointSet(CApp::get()->pWorld->getT());
	double v= s->getValue(m_sV);
	double u= a->getValue(m_aUThrust); //thrust
	double dot_v= (u*(-0.5*tanh((fabs((1.2+0.2*sin(fabs(v)))*v*fabs(v) - u) -30.0)*0.1) + 0.5) 
		- (1.2+0.2*sin(fabs(v)))*v*fabs(v))	/(3.0+1.5*sin(fabs(v)));
	double newV= v + dot_v*dt;

	s->setValue(m_sV,newV);
	s->setValue(m_sVSetpoint,newSetpoint);
	s->setValue(m_sVDeviation,newSetpoint-newV);
}
