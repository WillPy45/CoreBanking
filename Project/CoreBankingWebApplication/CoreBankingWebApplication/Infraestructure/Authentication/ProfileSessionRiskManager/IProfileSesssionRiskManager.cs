namespace CoreBankingWebApplication.Infraestructure.Authentication
{
    /// <summary>
    /// This can be manage stuff like: time to retry, ban the IP, manage in different way if is an admin for example, etc
    /// </summary>
    public interface IProfileSessionRiskManager
    {

        TrackedTrySessions SubscribeToFailAttempts(TrackedTrySession trackedTrySession);
        void UnSubscribeFromFailAttempt(TrackedTrySession trackedTrySession);

        /// <summary>
        /// Check if try session for that profile is locked
        /// </summary>
        /// <returns>
        /// A tuple indicating:
        /// <para><c>IsTriable</c>: true if the attempt is allowed; false otherwise.</para>
        /// <para><c>Details</c>: null if allowed; contains lock details if restricted.</para>
        /// </returns>
        (bool IsTriable, TrackedTrySessions? Details) CheckIfTriable(TrackedTrySession trackedTrySession);

    }
}
