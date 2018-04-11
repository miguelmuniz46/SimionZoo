#pragma once
#ifdef _WIN64

#ifdef BUILD_CNTK_WRAPPER
//we are building the CNTK Wrapper
#define DLL_API __declspec(dllexport)
#else
#define DLL_API
//#define CNTK_HEADERONLY_DEFINITIONS
#endif

#include <vector>

using namespace std;
namespace tinyxml2 { class XMLElement; }

#include "../../RLSimion/named-var-set.h"
class NetworkArchitecture;
class InputData;
class LinkConnection;
class OptimizerSettings;
class INetwork;
class IMinibatch;

//Interface class
class INetworkDefinition
{
public:
	virtual void destroy() = 0;

	virtual NetworkArchitecture* getNetworkArchitecture() = 0;
	virtual const LinkConnection* getOutput() const = 0;
	virtual const OptimizerSettings* getOptimizerSettings() const = 0;

	virtual void setInputLayerName(wstring name)= 0;
	virtual bool isInputLayer(wstring name)= 0;
	virtual void setOutputLayerName(wstring name)= 0;
	virtual wstring getOutputLayerName()= 0;

	virtual void addInputStateVar(size_t stateVarId) = 0;
	virtual size_t getNumInputStateVars() = 0; 
	virtual size_t getInputStateVar(size_t) = 0;
	virtual void setOutputAction(size_t actionVarId, size_t numOutputs, double minvalue, double maxvalue) = 0;
	virtual size_t getClosestOutputIndex(double value)=  0;
	virtual double getActionIndexOutput(size_t actionIndex) = 0;
	virtual size_t getOutputActionVar() = 0;
	virtual size_t getNumOutputs() = 0;

	virtual IMinibatch* createMinibatch(size_t size) = 0;

	//for convenience we override the learning rate set in the network definition's parameters
	virtual INetwork* createNetwork(double learningRate) = 0;
};

class INetwork
{
public:

	virtual void destroy()= 0;

	virtual void buildQNetwork(double learningRate)= 0;

	virtual void save(string fileName)= 0;

	virtual INetwork* getFrozenCopy() const= 0;

	virtual void train(IMinibatch* pMinibatch) = 0;
	virtual void get(const State* s, vector<double>& outputValues) = 0;

	virtual void train(std::unordered_map<std::string, std::vector<double>&>& inputDataMap
		, std::vector<double>& targetOutputData)= 0;
	virtual void predict(std::unordered_map<std::string, std::vector<double>&>& inputDataMap
		, std::vector<double>& predictionData)= 0;
};

class IMinibatch
{
public:
	virtual void destroy() = 0;

	virtual void clear() = 0;
	virtual void addTuple(const State* s, vector<double>& targetValues)= 0;
	virtual vector<double>& getInputVector()= 0;
	virtual vector<double>& getTargetOutputVector()= 0;
	virtual bool isFull() = 0;
};

namespace CNTKWrapper
{
	DLL_API INetworkDefinition* getProblemInstance(tinyxml2::XMLElement* pNode);
}

#endif