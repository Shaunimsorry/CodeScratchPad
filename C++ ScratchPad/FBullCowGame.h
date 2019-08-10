#pragma once
#include <string>;
using FString = std::string;
using int32 = int;

struct FBullCowCount
{
	int32 Bulls = 0;
	int32 Cows = 0;
};

enum class EWordStatus
{
	Ok,
	Not_Isogram,
	Wrong_Length,
	Not_Lowercase, 
};

class FBullCowGame 
{
public:
	FBullCowGame();
	int32 GetMaxTries() const;
	int32 GetCurrentTry();
	bool IsGameWon() const;
	void Reset();
	bool IsGuessValid(FString);
	FBullCowCount SubmitGuess(FString);
	FString checkHiddenWord();
	int32 GetHiddenWordLen() const;
	EWordStatus checkIsGuessValid(FString) const;
private:
	int32 MyCurrentTry;
	int32 MyMaxTries;
	FString MyHiddenWord;
	bool bGamewon;
};
