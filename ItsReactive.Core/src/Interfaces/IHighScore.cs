namespace ItsReactive.Core.Interfaces;

public interface IHighScore
{
    int Get(string gameName);
    void Set(string gameName, int score);
}