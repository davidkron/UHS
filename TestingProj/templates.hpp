#pragma once

template <typename T>
class templatedClass
{
	T templatedMember;
	int nonTemplatedMember = 10;

	void foo()
	{
		nonTemplatedMember++;
	}
};
