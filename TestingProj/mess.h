int global;
const int fynal;

void messy();

namespace boo
{

	void messier()
	{
		::messy();
		::messy();
	}

	int calc()
	{
		return fynal + 1;
	}
}
