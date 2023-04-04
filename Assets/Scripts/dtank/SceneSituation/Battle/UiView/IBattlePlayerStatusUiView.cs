namespace dtank
{
    public interface IBattlePlayerStatusUiView
    {
        void SetScore(int score);
        void SetRank(int rank);
        void SetHp(int hp);
        void SetDeadFlag(bool flag);
    }
}