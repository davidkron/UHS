#include "sample.hpp"
const float pi = 3.14f;


void printer::greet()
{
std::cout << GREET;
}

int main2()
{
printer p;
p.greet();
std::cout << "Press Enter to continue";
std::cin.get();
return 1;
}
