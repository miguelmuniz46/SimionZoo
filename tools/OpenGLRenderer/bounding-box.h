#pragma once

#include "vector3d.h"
#include "vector2d.h"

namespace tinyxml2 { class XMLElement; }

class BoundingBox3D
{
	bool m_bSet= false;
	Vector3D m_min, m_max;
public:
	BoundingBox3D();
	virtual ~BoundingBox3D();
	void addPoint(Vector3D p);
	void reset();

	Vector3D min() const;
	Vector3D max() const;
	Vector3D size() const;
	Vector3D center() const;

	void draw() const;
	void load(tinyxml2::XMLElement* pNode);
	bool bSet() const { return m_bSet; }
};

class BoundingBox2D
{
	bool m_bSet = false;
	Vector2D m_min, m_max;
public:
	BoundingBox2D();
	virtual ~BoundingBox2D();

	void addPoint(Vector2D p);
	void reset();

	Vector2D min() const;
	Vector2D max() const;
	Vector2D size() const;
	Vector2D center() const;

	void draw() const;
	void load(tinyxml2::XMLElement* pNode);
	bool bSet() const { return m_bSet; }
};
